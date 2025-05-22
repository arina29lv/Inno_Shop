using Moq;
using UserControl.Application.DTOs.UserDTOs;
using UserControl.Application.Handlers.UserHandlers;
using UserControl.Application.Queries.UserQueries;
using UserControl.Domain.Models;
using UserControl.UnitTests.UserTests.Base;

namespace UserControl.UnitTests.UserTests.Retrieval;

public class GetUserByIdTests : BaseUserRetrievalTests
{
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdTests()
    {
        _handler = new GetUserByIdHandler(UserRepositoryMock.Object, MapperMock.Object);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUserDto_WhenUserExists()
    {
        /*arrange*/
        var user = new User { Id = 1, Name = "user1" };
        var userDto = new UserDto { Name = "user1" };
        
        UserRepositoryMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);
        MapperMock.Setup(x => x.Map<UserDto>(user)).Returns(userDto);
        
        /*act*/
        var result = await _handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);
        
        /*assert*/
        Assert.NotNull(result);
        Assert.Equal("user1", result!.Name);
        UserRepositoryMock.Verify(x => x.GetUserByIdAsync(1), Times.Once);
        MapperMock.Verify(x => x.Map<UserDto>(user), Times.Once);
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExists()
    {
        /*arrange*/
        UserRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);
        
        /*act*/
        var result = await _handler.Handle(new GetUserByIdQuery(99), CancellationToken.None);
        
        /*assert*/
        Assert.Null(result);
        UserRepositoryMock.Verify(x => x.GetUserByIdAsync(99), Times.Once);
        MapperMock.Verify(x => x.Map<UserDto>(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnNull_WhenUserWithGiverIdDoesNotExists()
    {
        /*arrange*/
        var invalidId = 999;
        UserRepositoryMock.Setup(x => x.GetUserByIdAsync(invalidId)).ReturnsAsync((User?)null);
        
        /*act*/
        var result = await _handler.Handle(new GetUserByIdQuery(invalidId), CancellationToken.None);
        
        /*assert*/
        Assert.Null(result);
        UserRepositoryMock.Verify(x => x.GetUserByIdAsync(invalidId), Times.Once);
        MapperMock.Verify(x => x.Map<UserDto>(It.IsAny<int>()), Times.Never);
    }
}