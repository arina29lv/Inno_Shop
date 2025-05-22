using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Models;
using UserControl.UnitTests.AuthTests.Base;

namespace UserControl.UnitTests.AuthTests.Login;

public class LoginUserTests : BaseAuthTestWithEncryption
{
    private readonly LoginUserHandler _handler;

    public LoginUserTests()
    {
        _handler = new LoginUserHandler(
            TokenServiceMock.Object,
            UserRepositoryMock.Object,
            EncryptionServiceMock.Object
        );
    }
    
    [Fact]
    public async Task Handle_ShouldReturnLoginDto_WhenCredentialsAreValid()
    {
        /*arrange*/
        var user = new User
        {
            Email = "user@example.com",
            PasswordHash = "hashed-pwd",
            IsEmailConfirmed = true
        };

        UserRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
        EncryptionServiceMock.Setup(e => e.Verify("raw-password", "hashed-pwd")).Returns(true);
        TokenServiceMock.Setup(t => t.GenerateAccessToken(user)).Returns("access-token");
        TokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");

        var command = new LoginUserCommand { Email = user.Email, Password = "raw-password" };

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.NotNull(result);
        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);

        UserRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserNotFound()
    {
        /*arrange*/
        UserRepositoryMock.Setup(r => r.GetUserByEmailAsync("missing@example.com"))
            .ReturnsAsync((User?)null);

        var command = new LoginUserCommand
        {
            Email = "missing@example.com",
            Password = "any"
        };

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.Null(result);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenPasswordDoesNotMatch()
    {
        /*arrange*/
        var user = new User
        {
            Email = "user@example.com",
            PasswordHash = "correct-hash",
            IsEmailConfirmed = true
        };
        
        UserRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
        EncryptionServiceMock.Setup(e => e.Verify("wrong-password", "correct-hash")).Returns(false);

        var command = new LoginUserCommand { Email = user.Email, Password = "wrong-password" };

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.Null(result);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenEmailIsNotConfirmed()
    {
        /*arrange*/
        var user = new User
        {
            Email = "user@example.com",
            PasswordHash = "hash",
            IsEmailConfirmed = false
        };

        UserRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
        EncryptionServiceMock.Setup(e => e.Verify("pass", "hash")).Returns(true);

        var command = new LoginUserCommand { Email = user.Email, Password = "pass" };

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.Null(result);
        UserRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }
}