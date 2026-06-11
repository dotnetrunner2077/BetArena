using Applications.Layer.DTOs;
using Applications.Layer.Interfaces;
using Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Test.Api;

[TestFixture]
public class BetsControllerTests
{
    private Mock<IBetService> _betServiceMock = null!;
    private BetsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _betServiceMock = new Mock<IBetService>();
        _controller = new BetsController(_betServiceMock.Object);
    }

    [Test]
    public async Task PlaceBet_ValidRequest_Returns201Created()
    {
        var request = new PlaceBetRequest { UserId = 1, GameName = "Roulette", Stake = 100, WinAmount = 0 };
        var expectedResponse = new PlaceBetResponse
        {
            BetId = 1, UserId = 1, GameName = "Roulette",
            Stake = 100, WinAmount = 0, Result = "Lose", GameRtp = 0
        };
        _betServiceMock.Setup(s => s.PlaceBetAsync(request)).ReturnsAsync(expectedResponse);

        var result = await _controller.PlaceBet(request);

        var createdResult = result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult!.StatusCode, Is.EqualTo(201));
        Assert.That(createdResult.Value, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task PlaceBet_ArgumentException_Returns400BadRequest()
    {
        var request = new PlaceBetRequest { UserId = 1, GameName = "Roulette", Stake = 0, WinAmount = 0 };
        _betServiceMock.Setup(s => s.PlaceBetAsync(request))
            .ThrowsAsync(new ArgumentException("Stake amount must be greater than 0."));

        var result = await _controller.PlaceBet(request);

        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
        Assert.That(badRequest!.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task PlaceBet_GameNotFound_Returns404NotFound()
    {
        var request = new PlaceBetRequest { UserId = 1, GameName = "Unknown", Stake = 100, WinAmount = 0 };
        _betServiceMock.Setup(s => s.PlaceBetAsync(request))
            .ThrowsAsync(new InvalidOperationException("Game 'Unknown' not found."));

        var result = await _controller.PlaceBet(request);

        var notFound = result as NotFoundObjectResult;
        Assert.That(notFound, Is.Not.Null);
        Assert.That(notFound!.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task PlaceBet_UserNotFound_Returns404NotFound()
    {
        var request = new PlaceBetRequest { UserId = 999, GameName = "Roulette", Stake = 100, WinAmount = 0 };
        _betServiceMock.Setup(s => s.PlaceBetAsync(request))
            .ThrowsAsync(new InvalidOperationException("User with Id 999 not found."));

        var result = await _controller.PlaceBet(request);

        var notFound = result as NotFoundObjectResult;
        Assert.That(notFound, Is.Not.Null);
        Assert.That(notFound!.StatusCode, Is.EqualTo(404));
    }
}
