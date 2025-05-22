using Microsoft.Extensions.Configuration;
using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Interfaces;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.UnitTests.AuthTests.ResetPassword;

public class RequestPasswordResetHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly RequestPasswordResetHandler _handler;

    public RequestPasswordResetHandlerTests()
    {
        _handler = new RequestPasswordResetHandler(
            _userRepositoryMock.Object,
            _emailServiceMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldSendEmail_WhenUserExistsAndConfirmed()
    {
        var user = new User
        {
            Email = "test@example.com",
            IsEmailConfirmed = true
        };

        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);

        _userRepositoryMock.Setup(r => r.UpdateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        _emailServiceMock.Setup(e =>
            e.SendPasswordResetEmailAsync(user.Email, It.IsAny<string>())).Returns(Task.CompletedTask);

        var command = new RequestPasswordResetCommand { Email = user.Email };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.False(string.IsNullOrEmpty(user.PasswordResetToken));
        Assert.True(user.PasswordResetTokenExpiresAt > DateTime.UtcNow);

        _emailServiceMock.Verify(e =>
            e.SendPasswordResetEmailAsync(user.Email, It.Is<string>(link =>
                link.Contains(user.Email) && link.Contains(user.PasswordResetToken))), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync("notfound@example.com")).ReturnsAsync((User)null);

        var command = new RequestPasswordResetCommand { Email = "notfound@example.com" };
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenEmailNotConfirmed()
    {
        var user = new User { Email = "unconfirmed@example.com", IsEmailConfirmed = false };
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);

        var command = new RequestPasswordResetCommand { Email = user.Email };
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}