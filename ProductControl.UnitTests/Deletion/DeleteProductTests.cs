using Moq;
using ProductControl.Application.Command;
using ProductControl.Application.Handlers;
using ProductControl.Domain.Interfaces;
using ProductControl.Domain.Models;

namespace ProductControl.UnitTests.Deletion;

public class DeleteProductTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly DeleteProductHandler _handler;

    public DeleteProductTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new DeleteProductHandler(_productRepositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldDeleteProduct_WhenUserIsOwner()
    {
        /*arrange*/
        var product = new Product { Id = 1, UserId = 10 };
        var command = new DeleteProductCommand { Id = 1, UserId = 10, UserRole = "User" };

        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        _productRepositoryMock.Verify(r => r.DeleteProductAsync(product), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldDeleteProduct_WhenUserIsAdmin()
    {
        /*arrange*/
        var product = new Product { Id = 2, UserId = 99 };
        var command = new DeleteProductCommand { Id = 2, UserId = 123, UserRole = "Admin" };

        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(2)).ReturnsAsync(product);

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.True(result);
        _productRepositoryMock.Verify(r => r.DeleteProductAsync(product), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        /*arrange*/
        var command = new DeleteProductCommand { Id = 99, UserId = 10, UserRole = "User" };

        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(99)).ReturnsAsync((Product)null!);

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*assert*/
        Assert.False(result);
        _productRepositoryMock.Verify(r => r.DeleteProductAsync(It.IsAny<Product>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUserIsNotOwnerAndNotAdmin()
    {
        /*arrange*/
        var product = new Product { Id = 3, UserId = 50 };
        var command = new DeleteProductCommand { Id = 3, UserId = 99, UserRole = "User" };

        _productRepositoryMock.Setup(r => r.GetProductByIdAsync(3)).ReturnsAsync(product);

        /*act*/
        var result = await _handler.Handle(command, CancellationToken.None);

        /*arrange*/
        Assert.False(result);
        _productRepositoryMock.Verify(r => r.DeleteProductAsync(It.IsAny<Product>()), Times.Never);
    }
}