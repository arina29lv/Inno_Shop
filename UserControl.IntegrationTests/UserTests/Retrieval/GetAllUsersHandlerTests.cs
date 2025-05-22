using AutoMapper;
using UserControl.Application.DTOs.UserDTOs;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Application.Queries.UserQueries;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;

namespace UserControl.IntegrationTests.UserTests.Retrieval;

public class GetAllUsersHandlerTests : IntegrationTestBase
{
    private IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDto>());
        return new Mapper(config);
    }

    private GetAllUsersHandler CreateHandler()
    {
        var repo = new UserRepository(new UserDbContext(UserDbOptions));
        var mapper = CreateMapper();
        return new GetAllUsersHandler(repo, mapper);
    }

    private async Task SeedUsersAsync(params User[] users)
    {
        foreach (var user in users)
        {
            await SeedUserAsync(user);
        }
    }

    [Fact]
    public async Task Handle_ShouldReturnSeededUser()
    {
        /*arrange*/
        await SeedUsersAsync(new User { Name = "Alice", Email = "alice@mail.com", Role = "Admin", PasswordHash = "x" });
        var handler = CreateHandler();

        /*act*/
        var result = (await handler.Handle(new GetAllUsersQuery(), CancellationToken.None)).ToList();

        /*assert*/
        Assert.Single(result);
        Assert.Equal("alice@mail.com", result[0].Email);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        /*arrange*/
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        /*assert*/
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllUsers_WhenMultipleUsersExist()
    {
        /*arrange*/
        await SeedUsersAsync(
            new User { Name = "Alice", Email = "alice@mail.com", Role = "Admin", PasswordHash = "x" },
            new User { Name = "Bob", Email = "bob@mail.com", Role = "User", PasswordHash = "x" }
        );

        var handler = CreateHandler();

        /*act*/
        var result = (await handler.Handle(new GetAllUsersQuery(), CancellationToken.None)).ToList();

        /*assert*/
        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.Email == "alice@mail.com");
        Assert.Contains(result, u => u.Email == "bob@mail.com");
    }

    [Fact]
    public async Task Handle_ShouldMapUserToUserDto_Correctly()
    {
        /*arrange*/
        await SeedUserAsync(new User
        {
            Name = "Charlie",
            Email = "charlie@mail.com",
            Role = "Admin",
            PasswordHash = "x",
            IsActive = true
        });

        var handler = CreateHandler();

        /*act*/
        var result = (await handler.Handle(new GetAllUsersQuery(), CancellationToken.None)).ToList();
        var dto = result.Single();

        /*assert*/
        Assert.Equal("Charlie", dto.Name);
        Assert.Equal("charlie@mail.com", dto.Email);
        Assert.Equal("Admin", dto.Role);
    }
}