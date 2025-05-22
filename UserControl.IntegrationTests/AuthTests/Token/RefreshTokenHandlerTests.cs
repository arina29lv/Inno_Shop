using Microsoft.EntityFrameworkCore;
using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;
using Assert = Xunit.Assert;

namespace UserControl.IntegrationTests.AuthTests.Token;

public class RefreshTokenHandlerTests : IntegrationTestBase
{
     private readonly Mock<ITokenService> _tokenMock;

    public RefreshTokenHandlerTests()
    {
        _tokenMock = new Mock<ITokenService>();
    }

    private RefreshTokenHandler CreateHandler()
    {
        var repo = new UserRepository(new UserDbContext(UserDbOptions));
        return new RefreshTokenHandler(repo, _tokenMock.Object);
    }

    private async Task<User> CreateUserWithRefreshTokenAsync(string email, string token)
    {
        var user = new User
        {
            Name = "Token User",
            Email = email,
            Role = "User",
            PasswordHash = "x",
            IsEmailConfirmed = true,
            RefreshToken = token
        };

        await SeedUserAsync(user);
        return user;
    }

    [Fact]
    public async Task Handle_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        /*arrange*/
        var user = await CreateUserWithRefreshTokenAsync("refresh@mail.com", "old-refresh-token");

        _tokenMock.Setup(t => t.GenerateAccessToken(It.IsAny<User>())).Returns("new-access-token");
        _tokenMock.Setup(t => t.GenerateRefreshToken()).Returns("new-refresh-token");

        var handler = CreateHandler();

        var command = new RefreshTokenCommand { RefreshToken = "old-refresh-token" };

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.NotNull(result);
        Assert.Equal("new-access-token", result.AccessToken);
        Assert.Equal("new-refresh-token", result.RefreshToken);

        var updatedUser = await new UserDbContext(UserDbOptions)
            .Users.FirstAsync(u => u.Email == user.Email);

        Assert.Equal("new-refresh-token", updatedUser.RefreshToken);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRefreshTokenIsInvalid()
    {
        /*arrange*/
        var handler = CreateHandler();

        var command = new RefreshTokenCommand { RefreshToken = "invalid-token" };

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.Null(result);
    }
}