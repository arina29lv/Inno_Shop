using Moq;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.UnitTests.AuthTests.Base;

public class BaseAuthTestWithEncryption : BaseAuthTest
{
    protected readonly Mock<IEncryptionService> EncryptionServiceMock;

    protected BaseAuthTestWithEncryption()
    {
        EncryptionServiceMock = new Mock<IEncryptionService>();
    }
}