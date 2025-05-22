using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Models;
using UserControl.UnitTests.UserTests;
using UserControl.UnitTests.UserTests.Base;

namespace UserControl.UnitTests.AuthTests.ConfirmEmail;

public class ConfirmEmailTests : BaseUserTest
{ 
    private readonly ConfirmEmailHandler _handler;

    public ConfirmEmailTests()
    {
        _handler = new ConfirmEmailHandler(UserRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldConfirmEmail_WhenUserExistsAndTokenMatches()
    {
        /*arrange*/
        var user = new User
        {
            Email = "test@example.com",
            EmailConfirmationToken = "valid-token",
            IsEmailConfirmed = false
        };
        
        UserRepositoryMock
            .Setup(r => r.GetUserByEmailAsync("test@example.com"))
            .ReturnsAsync(user);

        var command = new ConfirmEmailCommand {Email = "test@example.com", Token = "valid-token"};
        
        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);
        
        /*assert*/
        Assert.True(result);
        Assert.True(user.IsEmailConfirmed);
        Assert.Null(user.EmailConfirmationToken);

        UserRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
    {
        /*arrange*/
        UserRepositoryMock
            .Setup(r => r.GetUserByEmailAsync("missing@example.com"))
            .ReturnsAsync((User)null);

        var command = new ConfirmEmailCommand { Email = "missing@example.com", Token = "any-token"};

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(result);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenTokenDoesNotMatch()
    {
        /*arrange*/
        var user = new User
        {
            Email = "test@example.com",
            EmailConfirmationToken = "correct-token",
            IsEmailConfirmed = false
        };

        UserRepositoryMock
            .Setup(r => r.GetUserByEmailAsync("test@example.com"))
            .ReturnsAsync(user);

        var command = new ConfirmEmailCommand {Email = "test@example.com", Token = "wrong-token"};

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(result);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }
}