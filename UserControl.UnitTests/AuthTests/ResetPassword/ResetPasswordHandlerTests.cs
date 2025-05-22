using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Interfaces;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.UnitTests.AuthTests.ResetPassword;

public class ResetPasswordHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEncryptionService> _encryptionServiceMock = new();
    private readonly ResetPasswordHandler _handler;

    public ResetPasswordHandlerTests()
    {
        _handler = new ResetPasswordHandler(_userRepositoryMock.Object, _encryptionServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldResetPassword_WhenTokenIsValid()
    {
        var user = new User
        {
            Email = "test@example.com",
            PasswordResetToken = "valid-token",
            PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
        _encryptionServiceMock.Setup(e => e.Hash("newpass")).Returns("hashed");

        _userRepositoryMock.Setup(r => r.UpdateUserAsync(user)).Returns(Task.CompletedTask);

        var command = new ResetPasswordCommand
        {
            Email = user.Email,
            Token = "valid-token",
            NewPassword = "newpass"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.Equal("hashed", user.PasswordHash);
        Assert.Null(user.PasswordResetToken);
        Assert.Null(user.PasswordResetTokenExpiresAt);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
    {
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync("notfound@example.com")).ReturnsAsync((User)null);

        var command = new ResetPasswordCommand
        {
            Email = "notfound@example.com",
            Token = "token",
            NewPassword = "pass"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenTokenIsInvalid()
    {
        var user = new User
        {
            Email = "test@example.com",
            PasswordResetToken = "expected-token",
            PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);

        var command = new ResetPasswordCommand
        {
            Email = user.Email,
            Token = "wrong-token",
            NewPassword = "pass"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenTokenExpired()
    {
        var user = new User
        {
            Email = "test@example.com",
            PasswordResetToken = "token",
            PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(-5) // expired
        };

        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);

        var command = new ResetPasswordCommand
        {
            Email = user.Email,
            Token = "token",
            NewPassword = "pass"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }
}