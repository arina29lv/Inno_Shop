using Moq;
using UserControl.Domain.Interfaces;

namespace UserControl.UnitTests.UserTests.Base;

public class BaseUserTest
{
    protected readonly Mock<IUserRepository> UserRepositoryMock;

    protected BaseUserTest()
    {
        UserRepositoryMock = new Mock<IUserRepository>();
    }
}