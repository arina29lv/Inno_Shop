using AutoMapper;
using Moq;

namespace UserControl.UnitTests.UserTests.Base;

public class BaseUserRetrievalTests : BaseUserTest
{
    protected readonly Mock<IMapper> MapperMock;

    public BaseUserRetrievalTests()
    {
        MapperMock = new Mock<IMapper>();
    }
}