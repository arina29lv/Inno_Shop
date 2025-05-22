using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;

namespace UserControl.IntegrationTests.AuthTests.ResetPassword;

public class RequestPasswordResetHandlerTests : IntegrationTestBase
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly IConfiguration _configuration;

    public RequestPasswordResetHandlerTests()
    {
        _emailServiceMock = new Mock<IEmailService>();

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "App:UserControlUrl", "http://localhost:5041" }
            })
            .Build();
    }

    private RequestPasswordResetHandler CreateHandler()
    {
        var db = new UserDbContext(UserDbOptions);
        var repo = new UserRepository(db);
        return new RequestPasswordResetHandler(repo, _emailServiceMock.Object, _configuration);
    }

    private async Task<User> CreateUserAsync(string email, bool isConfirmed)
    {
        var user = new User
        {
            Name = isConfirmed ? "Confirmed" : "Unconfirmed",
            Email = email,
            Role = "User",
            PasswordHash = "hashed",
            IsEmailConfirmed = isConfirmed
        };

        await SeedUserAsync(user);
        return user;
    }

    [Fact]
    public async Task Handle_ShouldSendResetLink_WhenEmailConfirmed()
    {
        /*arrange*/
        var user = await CreateUserAsync("confirmed@test.com", isConfirmed: true);

        string? capturedLink = null;
        _emailServiceMock
            .Setup(e => e.SendPasswordResetEmailAsync(user.Email, It.IsAny<string>()))
            .Callback<string, string>((_, link) => capturedLink = link)
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new RequestPasswordResetCommand { Email = user.Email }, CancellationToken.None);

        /*assert*/
        Assert.True(result);

        var updatedUser = await new UserDbContext(UserDbOptions)
            .Users.FirstAsync(u => u.Email == user.Email);

        Assert.NotNull(updatedUser.PasswordResetToken);
        Assert.True(updatedUser.PasswordResetTokenExpiresAt > DateTime.UtcNow);
        Assert.Contains(updatedUser.PasswordResetToken, capturedLink);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(new RequestPasswordResetCommand { Email = "missing@test.com" }, CancellationToken.None);

        Assert.False(result);
        _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenEmailIsNotConfirmed()
    {
        var user = await CreateUserAsync("unconfirmed@test.com", isConfirmed: false);

        var handler = CreateHandler();
        var result = await handler.Handle(new RequestPasswordResetCommand { Email = user.Email }, CancellationToken.None);

        Assert.False(result);
        _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}