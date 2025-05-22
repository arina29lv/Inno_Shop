using Moq;
using UserControl.Application.DTOs.UserDTOs;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Application.Queries.UserQueries;
using UserControl.Domain.Models;
using UserControl.UnitTests.UserTests.Base;

namespace UserControl.UnitTests.UserTests.Retrieval;

public class GetAllUsersTests : BaseUserRetrievalTests
{
    private readonly GetAllUsersHandler _handler;

    public GetAllUsersTests()
    {
        _handler = new GetAllUsersHandler(UserRepositoryMock.Object, MapperMock.Object);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllMappedUsers_WhenUsersExist()
    {
        /*arrange*/
        var users = new List<User>
        {
            new User { Id = 1, Name = "user1" },
            new User { Id = 2, Name = "user2" }
        };

        var userDtos = new List<UserDto>
        {
            new UserDto{ Name = "user1"},
            new UserDto{ Name = "user2"}
        };
        
        UserRepositoryMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);
        MapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);
        
        /*act*/
        var result = (await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None)).ToList();
        
        /*assert*/
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("user1", result[0].Name);
        Assert.Equal("user2", result[1].Name);
        
        UserRepositoryMock.Verify(r => r.GetAllUsersAsync(), Times.Once);
        MapperMock.Verify(m => m.Map<IEnumerable<UserDto>>(users), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnEmptyList_WhenUsersNotExist()
    {
        /*arrange*/
        var users = new List<User>();
        var userDtos = new List<UserDto>();
        
        UserRepositoryMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);
        MapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);
        
        /*act*/
        var result = (await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None)).ToList();
        
        /*assert*/
        Assert.Empty(result);
        UserRepositoryMock.Verify(r => r.GetAllUsersAsync(), Times.Once);
        MapperMock.Verify(m => m.Map<IEnumerable<UserDto>>(users), Times.Once);
    }
}