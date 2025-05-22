using Moq;
using ProductControl.Application.Command;
using ProductControl.Application.Handlers;
using ProductControl.Domain.Models;
using ProductControl.UnitTests.Base;

namespace ProductControl.UnitTests.Update;

public class UpdateProductTests : BaseProductTestWithMapper
{
    private readonly UpdateProductHandler _handler;

    public UpdateProductTests()
    {
        _handler = new UpdateProductHandler(ProductRepositoryMock.Object, MapperMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenUserIsOwner()
    {
        /*arrange*/
        var product = new Product { Id = 1, UserId = 10 };

        var command = new UpdateProductCommand
        {
            Id = 1,
            UpdatorId = 10,
            UpdatorRole = "User",
            Name = "Updated Name"
        };

        ProductRepositoryMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        MapperMock.Verify(m => m.Map(command, product), Times.Once);
        ProductRepositoryMock.Verify(r => r.UpdateProductAsync(product), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenUserIsAdmin()
    {
        /*arrange*/
        var product = new Product { Id = 2, UserId = 999 };

        var command = new UpdateProductCommand
        {
            Id = 2,
            UpdatorId = 1,
            UpdatorRole = "Admin",
            Name = "Admin Update"
        };

        ProductRepositoryMock.Setup(r => r.GetProductByIdAsync(2)).ReturnsAsync(product);

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        MapperMock.Verify(m => m.Map(command, product), Times.Once);
        ProductRepositoryMock.Verify(r => r.UpdateProductAsync(product), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProductNotFound()
    {
        /*arrange*/
        var command = new UpdateProductCommand
        {
            Id = 999,
            UpdatorId = 1,
            UpdatorRole = "Admin"
        };

        ProductRepositoryMock.Setup(r => r.GetProductByIdAsync(999)).ReturnsAsync((Product?)null);

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(result);
        MapperMock.Verify(m => m.Map(It.IsAny<UpdateProductCommand>(), It.IsAny<Product>()), Times.Never);
        ProductRepositoryMock.Verify(r => r.UpdateProductAsync(It.IsAny<Product>()), Times.Never);
    }
}