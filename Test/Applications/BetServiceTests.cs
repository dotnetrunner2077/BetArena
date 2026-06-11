using Applications.Layer.DTOs;
using Applications.Layer.Services;
using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Test.Applications;

[TestFixture]
public class BetServiceTests
{
    private BettingDbContext _context = null!;
    private BetService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BettingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new BettingDbContext(options);
        _service = new BetService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public void PlaceBetAsync_StakeIsZero_ThrowsArgumentException()
    {
        var request = new PlaceBetRequest { UserId = 1, GameName = "Roulette", Stake = 0, WinAmount = 0 };

        Assert.ThrowsAsync<ArgumentException>(() => _service.PlaceBetAsync(request));
    }

    [Test]
    public void PlaceBetAsync_StakeIsNegative_ThrowsArgumentException()
    {
        var request = new PlaceBetRequest { UserId = 1, GameName = "Roulette", Stake = -50, WinAmount = 0 };

        Assert.ThrowsAsync<ArgumentException>(() => _service.PlaceBetAsync(request));
    }

    [Test]
    public void PlaceBetAsync_WinAmountIsNegative_ThrowsArgumentException()
    {
        var request = new PlaceBetRequest { UserId = 1, GameName = "Roulette", Stake = 100, WinAmount = -10 };

        Assert.ThrowsAsync<ArgumentException>(() => _service.PlaceBetAsync(request));
    }

    [Test]
    public void PlaceBetAsync_GameNotFound_ThrowsInvalidOperationException()
    {
        _context.Users.Add(new User { Id = 1, Email = "user@test.com" });
        _context.SaveChanges();

        var request = new PlaceBetRequest { UserId = 1, GameName = "NonExistentGame", Stake = 100, WinAmount = 0 };

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.PlaceBetAsync(request));
    }

    [Test]
    public void PlaceBetAsync_UserNotFound_ThrowsInvalidOperationException()
    {
        _context.Games.Add(new Game { Id = 1, Description = "Roulette" });
        _context.SaveChanges();

        var request = new PlaceBetRequest { UserId = 999, GameName = "Roulette", Stake = 100, WinAmount = 0 };

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.PlaceBetAsync(request));
    }

    [Test]
    public async Task PlaceBetAsync_WinAmountIsZero_ResultIsLose()
    {
        _context.Games.Add(new Game { Id = 1, Description = "Roulette" });
        _context.Users.Add(new User { Id = 1, Email = "user@test.com" });
        await _context.SaveChangesAsync();

        var request = new PlaceBetRequest { UserId = 1, GameName = "Roulette", Stake = 100, WinAmount = 0 };

        var response = await _service.PlaceBetAsync(request);

        Assert.That(response.Result, Is.EqualTo("Lose"));
    }

    [Test]
    public async Task PlaceBetAsync_WinAmountGreaterThanZero_ResultIsWin()
    {
        _context.Games.Add(new Game { Id = 1, Description = "Blackjack" });
        _context.Users.Add(new User { Id = 1, Email = "user@test.com" });
        await _context.SaveChangesAsync();

        var request = new PlaceBetRequest { UserId = 1, GameName = "Blackjack", Stake = 100, WinAmount = 180 };

        var response = await _service.PlaceBetAsync(request);

        Assert.That(response.Result, Is.EqualTo("Win"));
    }

    [Test]
    public async Task PlaceBetAsync_ValidBet_RtpCalculatedCorrectly()
    {
        _context.Games.Add(new Game { Id = 1, Description = "Slots" });
        _context.Users.Add(new User { Id = 1, Email = "user@test.com" });
        await _context.SaveChangesAsync();

        // Place first bet: Stake=200, Win=100 → RTP = (100/200)*100 = 50.00
        var request = new PlaceBetRequest { UserId = 1, GameName = "Slots", Stake = 200, WinAmount = 100 };

        var response = await _service.PlaceBetAsync(request);

        Assert.That(response.GameRtp, Is.EqualTo(50.00m));
    }

    [Test]
    public async Task PlaceBetAsync_ValidBet_ResponseContainsExpectedFields()
    {
        _context.Games.Add(new Game { Id = 1, Description = "Poker" });
        _context.Users.Add(new User { Id = 1, Email = "user@test.com" });
        await _context.SaveChangesAsync();

        var request = new PlaceBetRequest { UserId = 1, GameName = "Poker", Stake = 150, WinAmount = 75 };

        var response = await _service.PlaceBetAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.UserId, Is.EqualTo(1));
            Assert.That(response.GameName, Is.EqualTo("Poker"));
            Assert.That(response.Stake, Is.EqualTo(150m));
            Assert.That(response.WinAmount, Is.EqualTo(75m));
            Assert.That(response.BetId, Is.GreaterThan(0));
        });
    }
}
