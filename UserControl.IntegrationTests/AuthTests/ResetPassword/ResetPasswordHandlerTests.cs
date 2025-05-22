using Microsoft.EntityFrameworkCore;
using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;

namespace UserControl.IntegrationTests.AuthTests.ResetPassword;

public class ResetPasswordHandlerTests : IntegrationTestBase
{
    private readonly Mock<IEncryptionService> _encryptionMock;

    public ResetPasswordHandlerTests()
    {
        _encryptionMock = new Mock<IEncryptionService>();
    }

    private ResetPasswordHandler CreateHandler()
    {
        var context = new UserDbContext(UserDbOptions);
        var repo = new UserRepository(context);
        return new ResetPasswordHandler(repo, _encryptionMock.Object);
    }

    private async Task<User> CreateUserAsync(string email, string token, DateTime? expiry, bool confirmed = true)
    {
        var user = new User
        {
            Email = email,
            Name = "Test User",
            Role = "User",
            PasswordHash = "old-hash",
            IsEmailConfirmed = confirmed,
            PasswordResetToken = token,
            PasswordResetTokenExpiresAt = expiry
        };

        await SeedUserAsync(user);
        return user;
    }

    private async Task<User> GetUserAsync(string email)
    {
        await using var context = new UserDbContext(UserDbOptions);
        return await context.Users.FirstAsync(u => u.Email == email);
    }

    [Fact]
    public async Task Handle_ShouldUpdatePassword_WhenTokenIsValid()
    {
        var user = await CreateUserAsync("valid@example.com", "valid-token", DateTime.UtcNow.AddMinutes(15));

        _encryptionMock.Setup(e => e.Hash("new-password")).Returns("new-hashed-password");
        var handler = CreateHandler();

        var command = new ResetPasswordCommand
        {
            Email = user.Email,
            Token = "valid-token",
            NewPassword = "new-password"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);

        var updatedUser = await GetUserAsync(user.Email);
        Assert.Equal("new-hashed-password", updatedUser.PasswordHash);
        Assert.Null(updatedUser.PasswordResetToken);
        Assert.Null(updatedUser.PasswordResetTokenExpiresAt);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenTokenIsInvalid()
    {
        await CreateUserAsync("wrong@example.com", "valid-token", DateTime.UtcNow.AddMinutes(15));

        var handler = CreateHandler();

        var command = new ResetPasswordCommand
        {
            Email = "wrong@example.com",
            Token = "wrong-token",
            NewPassword = "pass"
        };

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenTokenIsExpired()
    {
        await CreateUserAsync("expired@example.com", "expired-token", DateTime.UtcNow.AddMinutes(-5));

        var handler = CreateHandler();

        var command = new ResetPasswordCommand
        {
            Email = "expired@example.com",
            Token = "expired-token",
            NewPassword = "pass"
        };

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserNotFound()
    {
        var handler = CreateHandler();

        var command = new ResetPasswordCommand
        {
            Email = "notfound@example.com",
            Token = "token",
            NewPassword = "pass"
        };

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.False(result);
    }
}