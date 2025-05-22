using Moq;
using UserControl.Domain.Interfaces;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.UnitTests.AuthTests.Base;

public class BaseAuthTest
{
    protected readonly Mock<IUserRepository> UserRepositoryMock;
    protected readonly Mock<ITokenService> TokenServiceMock;

    public BaseAuthTest()
    {
        UserRepositoryMock = new Mock<IUserRepository>();
        TokenServiceMock = new Mock<ITokenService>();
    }
}