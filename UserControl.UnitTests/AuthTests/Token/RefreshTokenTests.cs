using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Models;
using UserControl.UnitTests.AuthTests.Base;

namespace UserControl.UnitTests.AuthTests.Token;

public class RefreshTokenTests : BaseAuthTest
{
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenTests()
    {
        _handler = new RefreshTokenHandler(UserRepositoryMock.Object, TokenServiceMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnLoginDto_WhenRefreshTokenIsValid()
    {
        /*arrange*/
        var user = new User { Id = 1, Email = "user@example.com", RefreshToken = "old-token" };

        UserRepositoryMock.Setup(r => r.GetUserByRefreshTokenAsync("old-token")).ReturnsAsync(user);
        TokenServiceMock.Setup(t => t.GenerateAccessToken(user)).Returns("new-access-token");
        TokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("new-refresh-token");

        var command = new RefreshTokenCommand { RefreshToken = "old-token" };

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.NotNull(result);
        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);
        Assert.Equal("new-refresh-token", user.RefreshToken);

        UserRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRefreshTokenIsInvalid()
    {
        /*arrange*/
        UserRepositoryMock.Setup(r => r.GetUserByRefreshTokenAsync("invalid-token")).ReturnsAsync((User)null);

        var command = new RefreshTokenCommand { RefreshToken = "invalid-token" };

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.Null(result);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        TokenServiceMock.Verify(t => t.GenerateAccessToken(It.IsAny<User>()), Times.Never);
    }
}