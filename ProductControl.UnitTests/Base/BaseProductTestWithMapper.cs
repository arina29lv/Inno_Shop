using AutoMapper;
using Moq;

namespace ProductControl.UnitTests.Base;

public class BaseProductTestWithMapper : BaseProductTest
{
    protected readonly Mock<IMapper> MapperMock;

    protected BaseProductTestWithMapper()
    {
        MapperMock = new Mock<IMapper>();
    }
}