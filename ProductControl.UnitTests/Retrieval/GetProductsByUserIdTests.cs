using Moq;
using ProductControl.Application.DTOs;
using ProductControl.Application.Handlers;
using ProductControl.Application.Queries;
using ProductControl.Domain.Models;
using ProductControl.UnitTests.Base;

namespace ProductControl.UnitTests.Retrieval;

public class GetProductsByUserIdTests : BaseProductTestWithMapper
{
    private readonly GetProductsByUserIdHandler _handler;

    public GetProductsByUserIdTests()
    {
        _handler = new GetProductsByUserIdHandler(ProductRepositoryMock.Object, MapperMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnMappedProducts_WhenProductsExist()
    {
        /*arrange*/
        var userId = 10;
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Milk" },
            new Product { Id = 2, Name = "Bread" }
        };
        var dtos = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Milk" },
            new ProductDto { Id = 2, Name = "Bread" }
        };

        ProductRepositoryMock.Setup(r => r.GetProductsByUserIdAsync(userId))
            .ReturnsAsync(products);

        MapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products))
            .Returns(dtos);

        var query = new GetProductsByUserIdQuery(userId);

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.Equal(2, result.Count());
        Assert.Equal("Milk", result.First().Name);
        MapperMock.Verify(m => m.Map<IEnumerable<ProductDto>>(products), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProductsExist()
    {
        /*arrange*/
        var products = new List<Product>();
        var dtos = new List<ProductDto>();

        ProductRepositoryMock.Setup(r => r.GetProductsByUserIdAsync(99))
            .ReturnsAsync(products);

        MapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products))
            .Returns(dtos);

        var query = new GetProductsByUserIdQuery(99);

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.Empty(result);
        MapperMock.Verify(m => m.Map<IEnumerable<ProductDto>>(products), Times.Once);
    }
}