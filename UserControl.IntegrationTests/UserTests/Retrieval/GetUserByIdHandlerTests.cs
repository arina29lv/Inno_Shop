using AutoMapper;
using UserControl.Application.DTOs.UserDTOs;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Application.Queries.UserQueries;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Persistence;
using UserControl.Infrastructure.Repositories;
using UserControl.IntegrationTests.Base;

namespace UserControl.IntegrationTests.UserTests.Retrieval;

public class GetUserByIdHandlerTests : IntegrationTestBase
{
    private IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDto>());
        return new Mapper(config);
    }

    private GetUserByIdHandler CreateHandler()
    {
        var repo = new UserRepository(new UserDbContext(UserDbOptions));
        var mapper = CreateMapper();
        return new GetUserByIdHandler(repo, mapper);
    }

    private async Task<User> CreateUserAsync(string name, string email, string role = "User")
    {
        var user = new User
        {
            Name = name,
            Email = email,
            Role = role,
            PasswordHash = "x"
        };
        await SeedUserAsync(user);
        return user;
    }

    [Fact]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        /*arrange*/
        var user = await CreateUserAsync("Alice", "alice@mail.com", "Admin");
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);

        /*assert*/
        Assert.NotNull(result);
        Assert.Equal(user.Email, result!.Email);
        Assert.Equal(user.Name, result.Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        /*arrange*/
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new GetUserByIdQuery(999), CancellationToken.None);

        /*assert*/
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldMapCorrectFields_WhenUserExists()
    {
        /*arrange*/
        var user = await CreateUserAsync("Bob", "bob@mail.com", "User");
        var handler = CreateHandler();

        /*act*/
        var result = await handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);

        /*assert*/
        Assert.NotNull(result);
        Assert.Equal("Bob", result!.Name);
        Assert.Equal("bob@mail.com", result.Email);
        Assert.Equal("User", result.Role);
    }
}