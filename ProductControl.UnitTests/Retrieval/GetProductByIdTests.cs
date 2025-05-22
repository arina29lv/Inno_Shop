using Moq;
using ProductControl.Application.DTOs;
using ProductControl.Application.Handlers;
using ProductControl.Application.Queries;
using ProductControl.Domain.Models;
using ProductControl.UnitTests.Base;

namespace ProductControl.UnitTests.Retrieval;
 
public class GetProductByIdTests : BaseProductTestWithMapper
{
    private readonly GetProductByIdHandler _handler;

    public GetProductByIdTests()
    {
        _handler = new GetProductByIdHandler(ProductRepositoryMock.Object, MapperMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenUserIsAdmin()
    {
        /*arrange*/
        var product = new Product { Id = 1, UserId = 10 };
        var dto = new ProductDto { Id = 1, Name = "AdminProduct" };

        ProductRepositoryMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);
        MapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(dto);

        var query = new GetProductByIdQuery(1) { UserId = 999, UserRole = "Admin" };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenUserIsOwner()
    {
        /*arrange*/
        var product = new Product { Id = 2, UserId = 5 };
        var dto = new ProductDto { Id = 2, Name = "OwnedProduct" };

        ProductRepositoryMock.Setup(r => r.GetProductByIdAsync(2)).ReturnsAsync(product);
        MapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(dto);

        var query = new GetProductByIdQuery(2) { UserId = 5, UserRole = "User" };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.NotNull(result);
        Assert.Equal(2, result!.Id);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserIsNotOwnerOrAdmin()
    {
        /*arrange*/
        var product = new Product { Id = 3, UserId = 10 };

        ProductRepositoryMock.Setup(r => r.GetProductByIdAsync(3)).ReturnsAsync(product);

        var query = new GetProductByIdQuery(3) { UserId = 99, UserRole = "User" };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.Null(result);
        MapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<Product>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductDoesNotExist()
    {
        /*arrange*/
        ProductRepositoryMock.Setup(r => r.GetProductByIdAsync(100)).ReturnsAsync((Product?)null);

        var query = new GetProductByIdQuery(100) { UserId = 1, UserRole = "Admin" };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.Null(result);
        MapperMock.Verify(m => m.Map<ProductDto>(It.IsAny<Product>()), Times.Never);
    }
}