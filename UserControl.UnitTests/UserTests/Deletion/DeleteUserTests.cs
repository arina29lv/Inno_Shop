using Moq;
using UserControl.Application.Commands.UserCommands;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Domain.Models;
using UserControl.UnitTests.UserTests.Base;

namespace UserControl.UnitTests.UserTests.Deletion;

public class DeleteUserTests : BaseUserTest
{
    private readonly DeleteUserHandler _handler;

    public DeleteUserTests()
    {
        _handler = new DeleteUserHandler(UserRepositoryMock.Object);
    }

    [Fact]
    public async Task DeleteUser_ShouldDeleteUser_WhenUserExists()
    {
        /*arrange*/
        var user = new User{ Id = 1 };
        UserRepositoryMock.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
        
        /*act*/
        var result = await _handler.Handle(new DeleteUserCommand { Id = user.Id }, CancellationToken.None);
        
        /*assert*/
        Assert.True(result);
        UserRepositoryMock.Verify(r => r.DeleteUserAsync(user), Times.Once);
    }
    
    [Fact]
    public async Task DeleteUser_ShouldReturnFalse_WhenUserDoesNotExists()
    {
        /*arrange*/
        UserRepositoryMock.Setup(r => r.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);
        
        /*act*/
        var result = await _handler.Handle(new DeleteUserCommand { Id = 99}, CancellationToken.None);
        
        /*assert*/
        Assert.False(result);
        UserRepositoryMock.Verify(r => r.DeleteUserAsync(It.IsAny<User>()), Times.Never);
    }
}