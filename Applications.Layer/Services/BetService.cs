using Applications.Layer.DTOs;
using Applications.Layer.Interfaces;
using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Applications.Layer.Services;

public class BetService : IBetService
{
    private readonly BettingDbContext _context;
    private readonly Dictionary<int, Bet> _betCache = new();
    private int _cacheKey = 0;

    public BetService(BettingDbContext context)
    {
        _context = context;
    }

    public async Task<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request)
    {
        if (request.Stake <= 0)
            throw new ArgumentException("Stake amount must be greater than 0.", nameof(request.Stake));

        if (request.WinAmount < 0)
            throw new ArgumentException("Win amount cannot be negative.", nameof(request.WinAmount));

        var game = await _context.Games
            .Include(g => g.Bets)
            .FirstOrDefaultAsync(g => g.Description == request.GameName)
            ?? throw new InvalidOperationException($"Game '{request.GameName}' not found.");

        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
            throw new InvalidOperationException($"User with Id {request.UserId} not found.");

        var bet = new Bet
        {
            UserId = request.UserId,
            GameId = game.Id,
            Amount = request.Stake,
            WinAmount = request.WinAmount,
            Result = request.WinAmount > 0 ? "Win" : "Lose"
        };

        _cacheKey++;
        _betCache[_cacheKey] = bet;

        var betLog = new Dictionary<string, object>
        {
            ["cacheKey"] = _cacheKey,
            ["userId"]   = bet.UserId,
            ["game"]     = game.Description,
            ["stake"]    = bet.Amount,
            ["winAmount"]= bet.WinAmount,
            ["result"]   = bet.Result
        };
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(betLog));

        _context.Bets.Add(bet);
        await _context.SaveChangesAsync();

        var allBetsForGame = game.Bets.ToList();
        allBetsForGame.Add(bet);

        decimal totalStake = allBetsForGame.Sum(b => b.Amount);
        decimal totalWin = allBetsForGame.Sum(b => b.WinAmount);
        game.Rtp = totalStake > 0 ? Math.Round(totalWin / totalStake * 100, 2) : 0m;

        await _context.SaveChangesAsync();

        return new PlaceBetResponse
        {
            BetId = bet.Id,
            UserId = bet.UserId,
            GameName = game.Description,
            Stake = bet.Amount,
            WinAmount = bet.WinAmount,
            Result = bet.Result,
            GameRtp = game.Rtp
        };
    }
}
