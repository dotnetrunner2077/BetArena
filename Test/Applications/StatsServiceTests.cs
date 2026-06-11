using Applications.Layer.Services;
using Domain.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Test.Applications;

[TestFixture]
public class StatsServiceTests
{
    private BettingDbContext _context = null!;
    private StatsService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BettingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new BettingDbContext(options);
        _service = new StatsService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetStatsAsync_NoData_ReturnsEmptyLists()
    {
        var result = await _service.GetStatsAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result.Games, Is.Empty);
            Assert.That(result.Users, Is.Empty);
        });
    }

    [Test]
    public async Task GetStatsAsync_GamesExist_ReturnsGamesWithRtp()
    {
        _context.Games.AddRange(
            new Game { Id = 1, Description = "Roulette", Rtp = 97.5m },
            new Game { Id = 2, Description = "Blackjack", Rtp = 99.0m }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetStatsAsync();

        Assert.That(result.Games, Has.Count.EqualTo(2));
        Assert.That(result.Games.First(g => g.Game == "Roulette").Rtp, Is.EqualTo(97.5m));
        Assert.That(result.Games.First(g => g.Game == "Blackjack").Rtp, Is.EqualTo(99.0m));
    }

    [Test]
    public async Task GetStatsAsync_BetsExist_ReturnsCorrectUserTotals()
    {
        _context.Games.Add(new Game { Id = 1, Description = "Slots" });
        _context.Users.Add(new User { Id = 1, Email = "user@test.com" });
        await _context.SaveChangesAsync();

        _context.Bets.AddRange(
            new Bet { GameId = 1, UserId = 1, Amount = 100, WinAmount = 50, Result = "Lose" },
            new Bet { GameId = 1, UserId = 1, Amount = 200, WinAmount = 350, Result = "Win" }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetStatsAsync();

        var userStats = result.Users.First(u => u.UserId == 1);
        Assert.Multiple(() =>
        {
            Assert.That(userStats.TotalStake, Is.EqualTo(300m));
            Assert.That(userStats.TotalWin, Is.EqualTo(400m));
        });
    }

    [Test]
    public async Task GetStatsAsync_NoBets_ReturnsEmptyUsersList()
    {
        _context.Games.Add(new Game { Id = 1, Description = "Poker" });
        await _context.SaveChangesAsync();

        var result = await _service.GetStatsAsync();

        Assert.That(result.Users, Is.Empty);
    }

    [Test]
    public async Task GetStatsAsync_MultipleUsers_EachUserHasOwnStats()
    {
        _context.Games.Add(new Game { Id = 1, Description = "Roulette" });
        _context.Users.AddRange(
            new User { Id = 1, Email = "user1@test.com" },
            new User { Id = 2, Email = "user2@test.com" }
        );
        await _context.SaveChangesAsync();

        _context.Bets.AddRange(
            new Bet { GameId = 1, UserId = 1, Amount = 100, WinAmount = 0, Result = "Lose" },
            new Bet { GameId = 1, UserId = 2, Amount = 300, WinAmount = 600, Result = "Win" }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetStatsAsync();

        Assert.That(result.Users, Has.Count.EqualTo(2));

        var user1 = result.Users.First(u => u.UserId == 1);
        var user2 = result.Users.First(u => u.UserId == 2);

        Assert.Multiple(() =>
        {
            Assert.That(user1.TotalStake, Is.EqualTo(100m));
            Assert.That(user1.TotalWin, Is.EqualTo(0m));
            Assert.That(user2.TotalStake, Is.EqualTo(300m));
            Assert.That(user2.TotalWin, Is.EqualTo(600m));
        });
    }
}
