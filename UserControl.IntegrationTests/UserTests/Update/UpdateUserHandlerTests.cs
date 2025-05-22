using AutoMapper;
using UserControl.Application.Commands.UserCommands;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;
using Assert = Xunit.Assert;

namespace UserControl.IntegrationTests.UserTests.Update;

public class UpdateUserHandlerTests : IntegrationTestBase
{
    private IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UpdateUserCommand, User>();
        });
        return new Mapper(config);
    }

    private UpdateUserHandler CreateHandler()
    {
        var repo = new UserRepository(new UserDbContext(UserDbOptions));
        var mapper = CreateMapper();
        return new UpdateUserHandler(repo, mapper);
    }

    private async Task<User> CreateUserAsync(string name, string email, string role = "User", string passwordHash = "x")
    {
        var user = new User
        {
            Name = name,
            Email = email,
            Role = role,
            PasswordHash = passwordHash
        };
        await SeedUserAsync(user);
        return user;
    }

    [Fact]
    public async Task Handle_ShouldUpdateUser_WhenUserExists()
    {
        /*arrange*/
        var user = await CreateUserAsync("Old Name", "old@mail.com", "User");
        var command = new UpdateUserCommand
        {
            Id = user.Id,
            Name = "New Name",
            Email = "new@mail.com",
            Role = "Admin"
        };
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        await using var context = new UserDbContext(UserDbOptions);
        var updated = await context.Users.FindAsync(user.Id);

        Assert.NotNull(updated);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal("new@mail.com", updated.Email);
        Assert.Equal("Admin", updated.Role);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
    {
        /*arrange*/
        var command = new UpdateUserCommand
        {
            Id = 999,
            Name = "Any",
            Email = "any@mail.com",
            Role = "Admin"
        };
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ShouldNotOverrideOtherFields_WhenUpdating()
    {
        /*arrange*/
        var user = await CreateUserAsync("User", "user@mail.com", "User", "original-hash");

        var command = new UpdateUserCommand
        {
            Id = user.Id,
            Name = "Updated User",
            Email = "updated@mail.com",
            Role = "Admin"
        };
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        await using var context = new UserDbContext(UserDbOptions);
        var updated = await context.Users.FindAsync(user.Id);

        Assert.Equal("Updated User", updated!.Name);
        Assert.Equal("updated@mail.com", updated.Email);
        Assert.Equal("Admin", updated.Role);
        Assert.Equal("original-hash", updated.PasswordHash);
    }
}