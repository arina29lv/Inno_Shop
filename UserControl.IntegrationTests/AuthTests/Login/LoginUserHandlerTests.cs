using Microsoft.EntityFrameworkCore;
using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Interfaces;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;

namespace UserControl.IntegrationTests.AuthTests.Login;

public class LoginUserHandlerTests : IntegrationTestBase
{ 
    private readonly Mock<IEncryptionService> _encryptionMock = new();
    private readonly Mock<ITokenService> _tokenMock = new();
    private readonly IUserRepository _userRepository;
    private LoginUserHandler _handler;

    public LoginUserHandlerTests()
    {
        var context = new UserDbContext(UserDbOptions);
        _userRepository = new UserRepository(context);
    }

    private void SetupHandler()
    {
        _handler = new LoginUserHandler(_tokenMock.Object, _userRepository, _encryptionMock.Object);
    }

    private async Task<User> CreateUserAsync(string email, string passwordHash, bool isConfirmed = true)
    {
        var user = new User
        {
            Name = "Test User",
            Email = email,
            Role = "User",
            PasswordHash = passwordHash,
            IsEmailConfirmed = isConfirmed
        };
        await SeedUserAsync(user);
        return user;
    }

    [Fact]
    public async Task Handle_ShouldReturnTokens_WhenLoginIsSuccessful()
    {
        var user = await CreateUserAsync("login@mail.com", "hashed");

        _encryptionMock.Setup(e => e.Verify("password", "hashed")).Returns(true);
        _tokenMock.Setup(t => t.GenerateAccessToken(It.IsAny<User>())).Returns("access-token");
        _tokenMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");

        SetupHandler();

        var command = new LoginUserCommand
        {
            Email = user.Email,
            Password = "password"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);

        var updated = await new UserDbContext(UserDbOptions)
            .Users.FirstAsync(u => u.Email == user.Email);

        Assert.Equal("refresh-token", updated.RefreshToken);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        SetupHandler();

        var result = await _handler.Handle(new LoginUserCommand
        {
            Email = "notfound@mail.com",
            Password = "pass"
        }, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenPasswordIsIncorrect()
    {
        var user = await CreateUserAsync("wrongpass@mail.com", "hashed");

        _encryptionMock.Setup(e => e.Verify("wrong", "hashed")).Returns(false);
        SetupHandler();

        var result = await _handler.Handle(new LoginUserCommand
        {
            Email = user.Email,
            Password = "wrong"
        }, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenEmailNotConfirmed()
    {
        var user = await CreateUserAsync("notconfirmed@mail.com", "hashed", isConfirmed: false);

        _encryptionMock.Setup(e => e.Verify("password", "hashed")).Returns(true);
        SetupHandler();

        var result = await _handler.Handle(new LoginUserCommand
        {
            Email = user.Email,
            Password = "password"
        }, CancellationToken.None);

        Assert.Null(result);
    }
}