using Moq;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.UnitTests.UserTests.Base;

public class BaseUserActivationTests : BaseUserTest
{
    protected readonly Mock<IEventBus> EventBusMock;

    protected BaseUserActivationTests()
    {
        EventBusMock = new Mock<IEventBus>();
    }
}