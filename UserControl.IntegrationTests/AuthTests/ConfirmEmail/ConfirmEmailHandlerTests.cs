using Microsoft.EntityFrameworkCore;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.Handlers.AuthHandlers;
using UserControl.Domain.Interfaces;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;

namespace UserControl.IntegrationTests.AuthTests.ConfirmEmail;

public class ConfirmEmailHandlerTests : IntegrationTestBase
{
    private readonly IUserRepository _userRepository;
    private readonly ConfirmEmailHandler _handler;

    public ConfirmEmailHandlerTests()
    {
        var context = new UserDbContext(UserDbOptions);
        _userRepository = new UserRepository(context);
        _handler = new ConfirmEmailHandler(_userRepository);
    }
    
    [Fact]
    public async Task Handle_ShouldConfirmEmail_WhenTokenIsValid()
    {
        var user = await CreateUserAsync("confirm@mail.com", "valid-token", isEmailConfirmed: false);

        var result = await _handler.Handle(new ConfirmEmailCommand
        {
            Email = user.Email,
            Token = "valid-token"
        }, CancellationToken.None);

        Assert.True(result);

        var confirmedUser = await GetUserByEmailAsync(user.Email);
        Assert.True(confirmedUser.IsEmailConfirmed);
        Assert.Null(confirmedUser.EmailConfirmationToken);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenTokenIsInvalid()
    {
        var user = await CreateUserAsync("wrong@mail.com", "valid-token", isEmailConfirmed: false);

        var result = await _handler.Handle(new ConfirmEmailCommand
        {
            Email = user.Email,
            Token = "invalid-token"
        }, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
    {
        var result = await _handler.Handle(new ConfirmEmailCommand
        {
            Email = "notfound@mail.com",
            Token = "any"
        }, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserFields_AfterConfirmation()
    {
        var user = await CreateUserAsync("confirm2@mail.com", "my-token", isEmailConfirmed: false);

        var result = await _handler.Handle(new ConfirmEmailCommand
        {
            Email = user.Email,
            Token = "my-token"
        }, CancellationToken.None);

        var updated = await GetUserByEmailAsync(user.Email);

        Assert.True(result);
        Assert.True(updated.IsEmailConfirmed);
        Assert.Null(updated.EmailConfirmationToken);
    }
    
    private async Task<User> CreateUserAsync(string email, string token, bool isEmailConfirmed)
    {
        var user = new User
        {
            Name = "Test User",
            Email = email,
            PasswordHash = "x",
            Role = "Admin",
            IsEmailConfirmed = isEmailConfirmed,
            EmailConfirmationToken = token
        };
        await SeedUserAsync(user);
        return user;
    }
    
    private async Task<User> GetUserByEmailAsync(string email)
    {
        await using var context = new UserDbContext(UserDbOptions);
        return await context.Users.FirstAsync(u => u.Email == email);
    }
}