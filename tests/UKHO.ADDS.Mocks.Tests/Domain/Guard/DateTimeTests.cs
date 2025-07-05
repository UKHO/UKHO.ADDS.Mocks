using UKHO.ADDS.Mocks.Domain.Guard;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Guard
{
    public sealed class DateTimeTests : BaseTests
    {
        [Theory(DisplayName = "DateTime: KindSpecified/KindUnspecified")]
        [InlineData(true)]
        [InlineData(false)]
        public void KindSpecified(bool testNull)
        {
            var specified = testNull ? default(DateTime?) : DateTime.UtcNow;
            var nullableSpecifiedArg = Mocks.Domain.Guard.Guard.Argument(() => specified).KindSpecified();

            var unspecified = testNull ? default(DateTime?) : default(DateTime);
            var nullableUnspecifiedArg = Mocks.Domain.Guard.Guard.Argument(() => unspecified).KindUnspecified();

            if (testNull)
            {
                nullableSpecifiedArg.KindUnspecified();
                nullableUnspecifiedArg.KindSpecified();
                return;
            }

            ThrowsArgumentException(
                nullableSpecifiedArg,
                arg => arg.KindUnspecified(),
                (arg, message) => arg.KindUnspecified(d =>
                {
                    Assert.Equal(specified, d);
                    return message;
                }));

            ThrowsArgumentException(
                nullableUnspecifiedArg,
                arg => arg.KindSpecified(),
                (arg, message) => arg.KindSpecified(d =>
                {
                    Assert.Equal(unspecified, d);
                    return message;
                }));

            var specifiedArg = Mocks.Domain.Guard.Guard.Argument(specified.Value, nameof(specified)).KindSpecified();
            var unspecifiedArg = Mocks.Domain.Guard.Guard.Argument(unspecified.Value, nameof(unspecified)).KindUnspecified();
            ThrowsArgumentException(
                specifiedArg,
                arg => arg.KindUnspecified(),
                (arg, message) => arg.KindUnspecified(d =>
                {
                    Assert.Equal(specified, d);
                    return message;
                }));

            ThrowsArgumentException(
                unspecifiedArg,
                arg => arg.KindSpecified(),
                (arg, message) => arg.KindSpecified(d =>
                {
                    Assert.Equal(unspecified, d);
                    return message;
                }));
        }
    }
}
