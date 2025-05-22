using Moq;
using UserControl.Application.Commands.UserCommands;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;

namespace UserControl.IntegrationTests.UserTests.Activation;

public class ActivateUserHandlerTests : IntegrationTestBase
{
    private readonly Mock<IEventBus> _eventBusMock;

    public ActivateUserHandlerTests()
    {
        _eventBusMock = new Mock<IEventBus>();
    }

    private ActivateUserHandler CreateHandler()
    {
        var repo = new UserRepository(new UserDbContext(UserDbOptions));
        return new ActivateUserHandler(repo, _eventBusMock.Object);
    }

    private async Task<User> CreateUserAsync(string email, bool isActive)
    {
        var user = new User
        {
            Name = isActive ? "Active User" : "Inactive User",
            Email = email,
            Role = "User",
            IsActive = isActive,
            PasswordHash = "x"
        };

        await SeedUserAsync(user);
        return user;
    }

    [Fact]
    public async Task Handle_ShouldActivateUser_WhenUserIsInactive()
    {
        /*arrange*/
        var user = await CreateUserAsync("inactive@mail.com", isActive: false);
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new ActivateUserCommand { UserId = user.Id }, CancellationToken.None);

        /*assert*/
        Assert.True(result);

        var updatedUser = await new UserDbContext(UserDbOptions).Users.FindAsync(user.Id);
        Assert.True(updatedUser!.IsActive);

        _eventBusMock.Verify(bus => bus.PublishUserActivated(user.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrueWithoutPublishing_WhenUserIsAlreadyActive()
    {
        /*arrange*/
        var user = await CreateUserAsync("active@mail.com", isActive: true);
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new ActivateUserCommand { UserId = user.Id }, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        _eventBusMock.Verify(bus => bus.PublishUserActivated(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
    {
        /*arrange*/
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new ActivateUserCommand { UserId = 999 }, CancellationToken.None);

        /*assert*/
        Assert.False(result);
        _eventBusMock.Verify(bus => bus.PublishUserActivated(It.IsAny<int>()), Times.Never);
    }
}