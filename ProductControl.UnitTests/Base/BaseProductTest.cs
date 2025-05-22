using Moq;
using ProductControl.Domain.Interfaces;

namespace ProductControl.UnitTests.Base;

public class BaseProductTest
{
    protected readonly Mock<IProductRepository> ProductRepositoryMock;

    protected BaseProductTest()
    {
        ProductRepositoryMock = new Mock<IProductRepository>();
    }
}