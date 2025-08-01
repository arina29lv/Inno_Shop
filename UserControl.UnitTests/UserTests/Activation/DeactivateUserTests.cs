using Moq;
using UserControl.Application.Commands.UserCommands;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Domain.Models;
using UserControl.UnitTests.UserTests.Base;

namespace UserControl.UnitTests.UserTests.Activation;

public class DeactivateUserTests : BaseUserActivationTests
{
    private readonly DeactivateUserHandler _handler;

    public DeactivateUserTests()
    {
        _handler = new DeactivateUserHandler(UserRepositoryMock.Object, EventBusMock.Object);
    }

    [Fact]
    public async Task DeactivateUser_ShouldReturnTrue_WhenUserIsActivated()
    {
        /*arrange*/
        var user = new User{ Id = 1, IsActive = true };
        UserRepositoryMock.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
        
        /*act*/
        var result = await _handler.Handle(new DeactivateUserCommand { UserId = user.Id}, CancellationToken.None);
        
        /*assert*/
        Assert.True(result);
        Assert.False(user.IsActive);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
        EventBusMock.Verify(e => e.PublishUserDeactivated(user.Id), Times.Once);
    }
    
    [Fact]
    public async Task DeactivateUser_ShouldReturnFalse_WhenUserNotFound()
    {
        /*arrange*/
        UserRepositoryMock.Setup(r => r.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);
        
        /*act*/
        var result = await _handler.Handle(new DeactivateUserCommand { UserId = 99}, CancellationToken.None);
        
        /*assert*/
        Assert.False(result);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        EventBusMock.Verify(e => e.PublishUserActivated(It.IsAny<int>()), Times.Never);
    }
    
    [Fact]
    public async Task DeactivateUser_ShouldReturnTrue_WhenUserIsAlreadyDeactivated()
    {
        /*arrange*/
        var user = new User{ Id = 1, IsActive = false };
        UserRepositoryMock.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
        
        /*act*/
        var result = await _handler.Handle(new DeactivateUserCommand { UserId = user.Id }, CancellationToken.None);
        
        /*assert*/
        Assert.True(result);
        Assert.False(user.IsActive);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Never);
        EventBusMock.Verify(e => e.PublishUserDeactivated(user.Id), Times.Never);
    }
}