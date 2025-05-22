using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;
using UserControl.UnitTests.AuthTests.Base;

namespace UserControl.UnitTests.AuthTests.Register;

public class RegisterUserTests : BaseAuthTestWithEncryption
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RegisterUserHandler _handler;
    private readonly Mock<IConfiguration> _configurationMock;

    public RegisterUserTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _mapperMock = new Mock<IMapper>();
        _configurationMock = new Mock<IConfiguration>();
        
        _handler = new RegisterUserHandler(
            UserRepositoryMock.Object,
            EncryptionServiceMock.Object,
            _emailServiceMock.Object,
            _configurationMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldRegisterUser_WhenEmailNotTaken()
    {
        /*arrange*/
        var command = new RegisterUserCommand
        {
            Email = "new@example.com",
            Password = "pass",
            Name = "User",
            Role = "User"
        };

        var user = new User { Email = command.Email, Name = command.Name, Role = command.Role };

        UserRepositoryMock.Setup(r => r.GetUserByEmailAsync(command.Email)).ReturnsAsync((User)null);
        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        EncryptionServiceMock.Setup(e => e.Hash(command.Password)).Returns("hashed-pass");
        UserRepositoryMock.Setup(r => r.AddUserAsync(user)).ReturnsAsync(123);

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.Equal((123, true), result);
        Assert.Equal("hashed-pass", user.PasswordHash);
        Assert.False(string.IsNullOrWhiteSpace(user.EmailConfirmationToken));

        _emailServiceMock.Verify(e =>
                e.SendConfirmationEmailAsync(
                    command.Email,
                    It.Is<string>(s => s.Contains(user.Email) && s.Contains(user.EmailConfirmationToken))
                ),
            Times.Once
        );
    }
    
    [Fact]
    public async Task Handle_ShouldReturnExistingUserId_WhenEmailAlreadyRegistered()
    {
        /*arrange*/
        var command = new RegisterUserCommand { Email = "existing@example.com" };
        var existingUser = new User { Id = 99, Email = command.Email };

        UserRepositoryMock.Setup(r => r.GetUserByEmailAsync(command.Email)).ReturnsAsync(existingUser);

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.Equal((99, false), result);

        UserRepositoryMock.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
        _emailServiceMock.Verify(e => e.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_ShouldGenerateConfirmationLink_WithCorrectTokenAndEmail()
    {
        /*arrange*/
        var command = new RegisterUserCommand { Email = "test@example.com", Password = "123", Name = "Test", Role = "User" };
        var user = new User { Email = command.Email, Name = command.Name, Role = command.Role };

        UserRepositoryMock.Setup(r => r.GetUserByEmailAsync(command.Email)).ReturnsAsync((User)null);
        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        EncryptionServiceMock.Setup(e => e.Hash(command.Password)).Returns("hashed");

        UserRepositoryMock.Setup(r => r.AddUserAsync(user)).ReturnsAsync(101);

        string? capturedLink = null;
        _emailServiceMock.Setup(e => e.SendConfirmationEmailAsync(command.Email, It.IsAny<string>()))
            .Callback<string, string>((_, link) => capturedLink = link);

        /*act*/
        await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.NotNull(capturedLink);
    }
    
    [Fact]
    public async Task Handle_ShouldHashPassword()
    {
        /*arrange*/
        var command = new RegisterUserCommand { Email = "hash@example.com", Password = "plain", Name = "N", Role = "R" };
        var user = new User { Email = command.Email };

        UserRepositoryMock.Setup(r => r.GetUserByEmailAsync(command.Email)).ReturnsAsync((User)null);
        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        EncryptionServiceMock.Setup(e => e.Hash("plain")).Returns("hashed-pass");
        UserRepositoryMock.Setup(r => r.AddUserAsync(user)).ReturnsAsync(10);

        /*act*/
        await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.Equal("hashed-pass", user.PasswordHash);
        Assert.NotEqual("plain", user.PasswordHash);
    }
}