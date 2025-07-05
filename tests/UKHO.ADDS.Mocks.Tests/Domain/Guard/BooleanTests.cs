using UKHO.ADDS.Mocks.Domain.Guard;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Guard
{
    public sealed class BooleanTests : BaseTests
    {
        [Theory(DisplayName = "Boolean: True/False")]
        [InlineData(null, null)]
        [InlineData(true, false)]
        public void GuardSupportsBooleans(bool? @true, bool? @false)
        {
            var nullableTrueArg = Mocks.Domain.Guard.Guard.Argument(() => @true).True();
            var nullableFalseArg = Mocks.Domain.Guard.Guard.Argument(() => @false).False();
            if (!@true.HasValue)
            {
                nullableTrueArg.False();
                nullableFalseArg.True();
                return;
            }

            ThrowsArgumentException(
                nullableFalseArg,
                arg => arg.True(),
                (arg, message) => arg.True(message));

            ThrowsArgumentException(
                nullableTrueArg,
                arg => arg.False(),
                (arg, message) => arg.False(message));

            var trueArg = Mocks.Domain.Guard.Guard.Argument(@true.Value, nameof(@true)).True();
            var falseArg = Mocks.Domain.Guard.Guard.Argument(@false.Value, nameof(@false)).False();
            ThrowsArgumentException(
                falseArg,
                arg => arg.True(),
                (arg, message) => arg.True(message));

            ThrowsArgumentException(
                trueArg,
                arg => arg.False(),
                (arg, message) => arg.False(message));
        }
    }
}
