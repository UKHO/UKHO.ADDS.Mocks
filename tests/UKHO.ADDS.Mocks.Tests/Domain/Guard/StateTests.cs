using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Guard
{
    public sealed class StateTests : BaseTests
    {
        [Fact(DisplayName = "State: Operation")]
        public void TestOperation()
        {
            Mocks.Domain.Guard.Guard.Operation(true);
            Mocks.Domain.Guard.Guard.Operation(true, RandomMessage);
            Mocks.Domain.Guard.Guard.Operation(true, RandomMessage, RandomMessage);

            var exceptions = ThrowsException<InvalidOperationException>(
                () => Mocks.Domain.Guard.Guard.Operation(false),
                message => Mocks.Domain.Guard.Guard.Operation(false, message));

            Assert.Contains(nameof(TestOperation), exceptions[0].Message);
        }

        [Fact(DisplayName = "State: Support")]
        public void TestSupport()
        {
            Mocks.Domain.Guard.Guard.Support(true);
            Mocks.Domain.Guard.Guard.Support(true, RandomMessage);
            Mocks.Domain.Guard.Guard.Support(true, RandomMessage, RandomMessage);

            var exceptions = ThrowsException<NotSupportedException>(
                () => Mocks.Domain.Guard.Guard.Support(false),
                message => Mocks.Domain.Guard.Guard.Support(false, message));

            Assert.Contains(nameof(TestSupport), exceptions[0].Message);
        }

        [Fact(DisplayName = "State: Disposal")]
        public void TestDisposal()
        {
            var objectName = RandomMessage;

            Mocks.Domain.Guard.Guard.Disposal(false);
            Mocks.Domain.Guard.Guard.Disposal(false, objectName);
            Mocks.Domain.Guard.Guard.Disposal(false, objectName, RandomMessage);

            var exceptions = ThrowsException<ObjectDisposedException>(
                () => Mocks.Domain.Guard.Guard.Disposal(true),
                message => Mocks.Domain.Guard.Guard.Disposal(true, message: message));

            Assert.Empty(exceptions[0].ObjectName);
            Assert.Empty(exceptions[1].ObjectName);

            exceptions = ThrowsException<ObjectDisposedException>(
                () => Mocks.Domain.Guard.Guard.Disposal(true, objectName),
                message => Mocks.Domain.Guard.Guard.Disposal(true, objectName, message));

            Assert.Same(objectName, exceptions[0].ObjectName);
            Assert.Same(objectName, exceptions[1].ObjectName);
        }

        private static TException[] ThrowsException<TException>(
            Action testWithoutMessage, Action<string> testWithMessage)
            where TException : Exception
        {
            var exWithoutMessage = Assert.Throws<TException>(() => testWithoutMessage());

            var message = RandomMessage;
            var exWithMessage = Assert.Throws<TException>(() => testWithMessage(message));
            Assert.StartsWith(message, exWithMessage.Message);

            return new[] { exWithoutMessage, exWithMessage };
        }
    }
}
