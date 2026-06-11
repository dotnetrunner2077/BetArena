using Applications.Layer.DTOs;
using Applications.Layer.Interfaces;
using Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Api;

[TestFixture]
public class StatsControllerTests
{
    private Mock<IStatsService> _statsServiceMock = null!;
    private StatsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _statsServiceMock = new Mock<IStatsService>();
        _controller = new StatsController(_statsServiceMock.Object);
    }

    [Test]
    public async Task GetStats_WithData_Returns200OkWithStatsResponse()
    {
        var expectedStats = new StatsResponse
        {
            Games = new List<GameStats>
            {
                new() { Game = "Roulette", Rtp = 97.5m },
                new() { Game = "Blackjack", Rtp = 99.0m }
            },
            Users = new List<UserStats>
            {
                new() { UserId = 1, TotalStake = 300m, TotalWin = 400m }
            }
        };
        _statsServiceMock.Setup(s => s.GetStatsAsync()).ReturnsAsync(expectedStats);

        var result = await _controller.GetStats();

        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(expectedStats));
    }

    [Test]
    public async Task GetStats_NoData_Returns200OkWithEmptyLists()
    {
        var emptyStats = new StatsResponse();
        _statsServiceMock.Setup(s => s.GetStatsAsync()).ReturnsAsync(emptyStats);

        var result = await _controller.GetStats();

        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));

        var stats = okResult.Value as StatsResponse;
        Assert.Multiple(() =>
        {
            Assert.That(stats!.Games, Is.Empty);
            Assert.That(stats.Users, Is.Empty);
        });
    }
}
