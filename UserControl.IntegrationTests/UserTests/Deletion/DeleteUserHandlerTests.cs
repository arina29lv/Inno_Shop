using UserControl.Application.Commands.UserCommands;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;

namespace UserControl.IntegrationTests.UserTests.Deletion;

public class DeleteUserHandlerTests : IntegrationTestBase
{
    private DeleteUserHandler CreateHandler()
    {
        var repo = new UserRepository(new UserDbContext(UserDbOptions));
        return new DeleteUserHandler(repo);
    }

    private async Task<User> CreateUserAsync(string email)
    {
        var user = new User
        {
            Name = "ToDelete",
            Email = email,
            Role = "User",
            PasswordHash = "x"
        };

        await SeedUserAsync(user);
        return user;
    }

    private async Task<User?> GetUserByIdAsync(int userId)
    {
        await using var context = new UserDbContext(UserDbOptions);
        return await context.Users.FindAsync(userId);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExists()
    {
        /*arrange*/
        var user = await CreateUserAsync("delete@mail.com");
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new DeleteUserCommand { Id = user.Id }, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        var deletedUser = await GetUserByIdAsync(user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
    {
        /*arrange*/
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new DeleteUserCommand { Id = 999 }, CancellationToken.None);

        /*assert*/
        Assert.False(result);
    }
}