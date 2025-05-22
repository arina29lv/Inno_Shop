using Microsoft.EntityFrameworkCore;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Persistence;
using Xunit;

namespace UserControl.IntegrationTests.Base;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly DbContextOptions<UserDbContext> UserDbOptions;

    public IntegrationTestBase()
    {
        UserDbOptions = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase("UserTestDb") 
            .Options;
    }

    public async Task InitializeAsync()
    {
        await using var db = new UserDbContext(UserDbOptions);
        db.Users.RemoveRange(db.Users); 
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected async Task SeedUserAsync(User user)
    {
        await using var db = new UserDbContext(UserDbOptions);
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}