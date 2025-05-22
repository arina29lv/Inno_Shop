using Moq;
using UserControl.Application.Commands.UserCommands;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Domain.Models;
using UserControl.UnitTests.UserTests.Base;

namespace UserControl.UnitTests.UserTests.Update;

public class UpdateUserTests : BaseUserRetrievalTests
{
    private readonly UpdateUserHandler _handler;

    public UpdateUserTests()
    {
        _handler = new UpdateUserHandler(UserRepositoryMock.Object, MapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUser_WhenUserExists()
    {
        /*arrange*/
        var user = new User { Id = 1, Name = "Old Name" };
        var command = new UpdateUserCommand { Id = 1, Name = "New Name" };
        
        UserRepositoryMock.Setup(r => r.GetUserByIdAsync(command.Id)).ReturnsAsync(user);
        MapperMock.Setup(m => m.Map(command, user));
        
        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        UserRepositoryMock.Verify(r => r.GetUserByIdAsync(command.Id), Times.Once);
        MapperMock.Verify(m => m.Map(command, user), Times.Once);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
    {
        /*arrange*/
        var command = new UpdateUserCommand { Id = 999, Name = "Test" };
        UserRepositoryMock.Setup(r => r.GetUserByIdAsync(command.Id)).ReturnsAsync((User?)null);
        
        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(result);
        UserRepositoryMock.Verify(r => r.GetUserByIdAsync(command.Id), Times.Once);
        MapperMock.Verify(m => m.Map(It.IsAny<UpdateUserCommand>(), It.IsAny<User>()), Times.Never);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }
}