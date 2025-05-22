using Moq;
using UserControl.Application.Commands.UserCommands;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Domain.Models;
using UserControl.UnitTests.UserTests.Base;

namespace UserControl.UnitTests.UserTests.Activation;

public class ActivateUserTests : BaseUserActivationTests
{
    private readonly ActivateUserHandler _handler;

    public ActivateUserTests()
    {
        _handler = new ActivateUserHandler(UserRepositoryMock.Object, EventBusMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldActivateUser_WhenUserExists()
    {
        /*arrange*/
        var user = new User{ Id = 1, IsActive = false };
        UserRepositoryMock.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        
        /*act*/
        var result = await _handler.Handle(new ActivateUserCommand { UserId = user.Id}, CancellationToken.None);
        
        /*assert*/
        Assert.True(result);
        Assert.True(user.IsActive);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
        EventBusMock.Verify(e => e.PublishUserActivated(user.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
    {
        /*arrange*/
        UserRepositoryMock.Setup(r => r.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);
        
        /*act*/
        var result = await _handler.Handle(new ActivateUserCommand { UserId = 99}, CancellationToken.None);
        
        /*assert*/
        Assert.False(result);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        EventBusMock.Verify(e => e.PublishUserActivated(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenUserIsAlreadyActive()
    {
        /*arrange*/
        var user = new User{ Id = 1, IsActive = true };
        UserRepositoryMock.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
        
        /*act*/
        var result = await _handler.Handle(new ActivateUserCommand { UserId = user.Id}, CancellationToken.None);
        
        /*assert*/
        Assert.True(result);
        Assert.True(user.IsActive);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Never);
        EventBusMock.Verify(e => e.PublishUserActivated(user.Id), Times.Never);
    }
}