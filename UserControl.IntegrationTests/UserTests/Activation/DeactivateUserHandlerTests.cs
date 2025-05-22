using Moq;
using UserControl.Application.Commands.UserCommands;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;

namespace UserControl.IntegrationTests.UserTests.Activation;

public class DeactivateUserHandlerTests : IntegrationTestBase
{ 
    private readonly Mock<IEventBus> _eventBusMock;

    public DeactivateUserHandlerTests()
    {
        _eventBusMock = new Mock<IEventBus>();
    }

    private DeactivateUserHandler CreateHandler()
    {
        var repo = new UserRepository(new UserDbContext(UserDbOptions));
        return new DeactivateUserHandler(repo, _eventBusMock.Object);
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
    public async Task Handle_ShouldDeactivateUser_AndPublishEvent_WhenUserIsActive()
    {
        /*arrange*/
        var user = await CreateUserAsync("deactivate@mail.com", isActive: true);
        var handler = CreateHandler();
        
        /*act*/
        var result = await handler.Handle(new DeactivateUserCommand { UserId = user.Id }, CancellationToken.None);

        /*assert*/
        Assert.True(result);

        var updatedUser = await new UserDbContext(UserDbOptions).Users.FindAsync(user.Id);
        Assert.False(updatedUser!.IsActive);

        _eventBusMock.Verify(b => b.PublishUserDeactivated(user.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrueWithoutPublishing_WhenUserAlreadyInactive()
    {
        /*arrange*/
        var user = await CreateUserAsync("inactive@mail.com", isActive: false);
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new DeactivateUserCommand { UserId = user.Id }, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        _eventBusMock.Verify(b => b.PublishUserDeactivated(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
    {
        /*arrange*/
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new DeactivateUserCommand { UserId = 999 }, CancellationToken.None);

        /*assert*/
        Assert.False(result);
        _eventBusMock.Verify(b => b.PublishUserDeactivated(It.IsAny<int>()), Times.Never);
    }
}