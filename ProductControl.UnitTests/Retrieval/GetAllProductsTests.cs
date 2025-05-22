using Moq;
using ProductControl.Application.DTOs;
using ProductControl.Application.Handlers;
using ProductControl.Application.Queries;
using ProductControl.Domain.Models;
using ProductControl.UnitTests.Base;

namespace ProductControl.UnitTests.Retrieval;

public class GetAllProductsTests : BaseProductTestWithMapper
{
    private readonly GetAllProductsHandler _handler;

    public GetAllProductsTests()
    {
        _handler = new GetAllProductsHandler(ProductRepositoryMock.Object, MapperMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnAllProducts_WhenUserIsAdmin()
    {
        /*arrange*/
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "A" },
            new Product { Id = 2, Name = "B" }
        };

        ProductRepositoryMock.Setup(r => r.GetAllProductsAsync())
            .ReturnsAsync(products);

        MapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products))
            .Returns(products.Select(p => new ProductDto { Id = p.Id, Name = p.Name }));

        var query = new GetAllProductsQuery { UserRole = "Admin" };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.Equal(2, result.Count());
        ProductRepositoryMock.Verify(r => r.GetAllProductsAsync(), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnUserProducts_WhenUserIsNotAdmin()
    {
        /*arrange*/
        var products = new List<Product> { new Product { Id = 1, UserId = 5 } };

        ProductRepositoryMock.Setup(r => r.GetProductsByUserIdAsync(5))
            .ReturnsAsync(products);

        MapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products))
            .Returns(products.Select(p => new ProductDto { Id = p.Id }));

        var query = new GetAllProductsQuery { UserId = 5, UserRole = "User" };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.Single(result);
        ProductRepositoryMock.Verify(r => r.GetProductsByUserIdAsync(5), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldApplyAllFilters()
    {
        /*arrange*/
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Milk", IsAvailable = true, Price = 10 },
            new Product { Id = 2, Name = "Bread", IsAvailable = false, Price = 5 },
            new Product { Id = 3, Name = "Almond Milk", IsAvailable = true, Price = 15 }
        };

        ProductRepositoryMock.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(products);
        
        MapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(It.Is<IEnumerable<Product>>(p => p.Count() == 1)))
            .Returns(new List<ProductDto> { new ProductDto { Id = 3, Name = "Almond Milk" } });

        var query = new GetAllProductsQuery
        {
            UserRole = "Admin",
            IsAvailable = true,
            PriceMin = 11,
            PriceMax = 20,
            NameContains = "milk"
        };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.Single(result);
        Assert.Equal("Almond Milk", result.First().Name);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProductsFound()
    {
        /*arrange*/
        ProductRepositoryMock.Setup(r => r.GetProductsByUserIdAsync(5)).ReturnsAsync(new List<Product>());
        MapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
            .Returns(new List<ProductDto>());

        var query = new GetAllProductsQuery { UserId = 5, UserRole = "User" };

        /*act*/
        var result = await _handler.Handle(query, CancellationToken.None);

        /*assert*/
        Assert.Empty(result);
    }
}