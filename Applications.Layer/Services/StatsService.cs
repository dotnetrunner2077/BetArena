using Applications.Layer.DTOs;
using Applications.Layer.Interfaces;
using Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Applications.Layer.Services;

public class StatsService : IStatsService
{
    private readonly BettingDbContext _context;

    public StatsService(BettingDbContext context)
    {
        _context = context;
    }

    public async Task<StatsResponse> GetStatsAsync()
    {
        var gameStats = await _context.Games
            .Select(g => new GameStats
            {
                Game = g.Description,
                Rtp = g.Rtp
            })
            .ToListAsync();

        var userStats = await _context.Bets
            .GroupBy(b => b.UserId)
            .Select(g => new UserStats
            {
                UserId = g.Key,
                TotalStake = g.Sum(b => b.Amount),
                TotalWin = g.Sum(b => b.WinAmount)
            })
            .ToListAsync();

        return new StatsResponse
        {
            Games = gameStats,
            Users = userStats
        };
    }
}
