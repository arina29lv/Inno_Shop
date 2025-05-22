using Moq;
using ProductControl.Application.Command;
using ProductControl.Application.Handlers;
using ProductControl.Domain.Models;
using ProductControl.UnitTests.Base;

namespace ProductControl.UnitTests.Activation;

public class ActivateProductsTests : BaseProductTest
{
    private readonly ActivateProductsHandler _handler;

    public ActivateProductsTests()
    {
        _handler = new ActivateProductsHandler(ProductRepositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldActivateAllProducts_WhenProductsExist()
    {
        /*arrange*/
        var products = new List<Product>
        {
            new Product { Id = 1, IsDeleted = true },
            new Product { Id = 2, IsDeleted = true }
        };

        ProductRepositoryMock.Setup(r => r.GetProductsByUserIdAsync(10))
            .ReturnsAsync(products);

        var command = new ActivateProductsCommand {UserId = 10};

        /*act*/
        await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.All(products, p => Assert.False(p.IsDeleted));
        ProductRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldDoNothing_WhenNoProductsExist()
    {
        /*arrange*/
        ProductRepositoryMock.Setup(r => r.GetProductsByUserIdAsync(20))
            .ReturnsAsync(new List<Product>());

        var command = new ActivateProductsCommand {UserId = 20};

        /*act*/
        await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        ProductRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}