using Moq;
using ProductControl.Application.Command;
using ProductControl.Application.Handlers;
using ProductControl.Domain.Models;
using ProductControl.UnitTests.Base;

namespace ProductControl.UnitTests.Activation;

public class DeactivateProductsTests : BaseProductTest
{
    private readonly DeactivateProductsHandler _handler;

    public DeactivateProductsTests()
    {
        _handler = new DeactivateProductsHandler(ProductRepositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldDeactivateAllProducts_WhenProductsExist()
    {
        /*arrange*/
        var products = new List<Product>
        {
            new Product { Id = 1, IsDeleted = false },
            new Product { Id = 2, IsDeleted = false }
        };

        ProductRepositoryMock
            .Setup(r => r.GetProductsByUserIdAsync(42))
            .ReturnsAsync(products);

        var command = new DeactivateProductsCommand {UserId = 42};

        /*act*/
        await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.All(products, p => Assert.True(p.IsDeleted));
        ProductRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldNotFail_WhenNoProductsExist()
    {
        /*arrange*/
        ProductRepositoryMock
            .Setup(r => r.GetProductsByUserIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Product>());

        var command = new DeactivateProductsCommand {UserId = 99};

        /*act*/
        await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        ProductRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}