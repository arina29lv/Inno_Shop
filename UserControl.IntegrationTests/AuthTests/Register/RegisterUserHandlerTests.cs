using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;
using Assert = Xunit.Assert;

namespace UserControl.IntegrationTests.AuthTests.Register;

public class RegisterUserHandlerTests : IntegrationTestBase
{
    private readonly Mock<IEncryptionService> _encryptionMock = new();
    private readonly Mock<IEmailService> _emailMock = new();
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public RegisterUserHandlerTests()
    {
        _config = CreateTestConfig();
        _mapper = SetupMapper();
    }

    private RegisterUserHandler CreateHandler()
    {
        var repo = new UserRepository(new UserDbContext(UserDbOptions));
        return new RegisterUserHandler(repo, _encryptionMock.Object, _emailMock.Object, _config, _mapper);
    }

    private static IConfiguration CreateTestConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "App:FrontendBaseUrl", "http://test.localhost" }
            })
            .Build();
    }

    private static IMapper SetupMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<RegisterUserCommand, User>();
        });
        return new Mapper(config);
    }

    [Fact]
    public async Task Handle_ShouldRegisterUser_AndSendConfirmationEmail_WhenUserIsNew()
    {
        /*arrange*/
        var command = new RegisterUserCommand
        {
            Name = "New User",
            Email = "newuser@mail.com",
            Password = "SecurePass123!",
            Role = "User"
        };

        _encryptionMock.Setup(e => e.Hash(It.IsAny<string>())).Returns("hashed-password");
        _emailMock.Setup(e => e.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var handler = CreateHandler();

        /*act*/
        var (id, isRegistered) = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(isRegistered);
        Assert.True(id > 0);

        await using var context = new UserDbContext(UserDbOptions);
        var user = await context.Users.FindAsync(id);

        Assert.NotNull(user);
        Assert.Equal("New User", user!.Name);
        Assert.Equal("newuser@mail.com", user.Email);
        Assert.Equal("User", user.Role);
        Assert.Equal("hashed-password", user.PasswordHash);
        Assert.False(user.IsEmailConfirmed);
        Assert.NotNull(user.EmailConfirmationToken);

        _emailMock.Verify(e => e.SendConfirmationEmailAsync(
            user.Email,
            It.Is<string>(link => link.Contains(user.EmailConfirmationToken!))
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserAlreadyExists()
    {
        /*arrange*/
        var existingUser = new User
        {
            Name = "Existing",
            Email = "existing@mail.com",
            Role = "User",
            PasswordHash = "x",
            IsEmailConfirmed = true
        };
        await SeedUserAsync(existingUser);

        var command = new RegisterUserCommand
        {
            Name = "Another User",
            Email = existingUser.Email,
            Password = "AnotherPassword",
            Role = "User"
        };

        var handler = CreateHandler();

        /*act*/
        var (id, isRegistered) = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(isRegistered);
        Assert.Equal(existingUser.Id, id);

        _emailMock.Verify(e => e.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}