using UKHO.ADDS.Mocks.Domain.Guard;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Guard
{
    public sealed class DoubleTests : BaseTests
    {
        [Theory(DisplayName = "Double: NaN/NotNaN")]
        [InlineData(null, null)]
        [InlineData(double.NaN, double.NegativeInfinity)]
        [InlineData(double.NaN, -1.0)]
        [InlineData(double.NaN, 0.0)]
        [InlineData(double.NaN, 1.0)]
        [InlineData(double.NaN, double.PositiveInfinity)]
        public void NaN(double? nan, double? nonNaN)
        {
            var nullableNaNArg = Mocks.Domain.Guard.Guard.NaN(Mocks.Domain.Guard.Guard.Argument(() => nan));
            var nullableNonNaNArg = Mocks.Domain.Guard.Guard.NotNaN(Mocks.Domain.Guard.Guard.Argument(() => nonNaN));
            if (!nan.HasValue)
            {
                Mocks.Domain.Guard.Guard.NotNaN(nullableNaNArg);
                Mocks.Domain.Guard.Guard.NaN(nullableNonNaNArg);
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableNonNaNArg,
                arg => Mocks.Domain.Guard.Guard.NaN(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NaN(arg, d =>
                {
                    Assert.Equal(nonNaN, d);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                nullableNaNArg,
                arg => Mocks.Domain.Guard.Guard.NotNaN(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotNaN(arg, message));

            var nanArg = Mocks.Domain.Guard.Guard.NaN(Mocks.Domain.Guard.Guard.Argument(nan.Value, nameof(nan)));
            var nonNaNArg = Mocks.Domain.Guard.Guard.NotNaN(Mocks.Domain.Guard.Guard.Argument(nonNaN.Value, nameof(nonNaN)));
            ThrowsArgumentOutOfRangeException(
                nonNaNArg,
                arg => Mocks.Domain.Guard.Guard.NaN(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NaN(arg, d =>
                {
                    Assert.Equal(nonNaN, d);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                nanArg,
                arg => Mocks.Domain.Guard.Guard.NotNaN(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotNaN(arg, message));
        }

        [Theory(DisplayName = "Double: Infinity/NotInfinity")]
        [InlineData(null, null)]
        [InlineData(double.NegativeInfinity, double.NaN)]
        [InlineData(double.NegativeInfinity, -1.0)]
        [InlineData(double.NegativeInfinity, 0.0)]
        [InlineData(double.NegativeInfinity, 1.0)]
        [InlineData(double.PositiveInfinity, double.NaN)]
        [InlineData(double.PositiveInfinity, -1.0)]
        [InlineData(double.PositiveInfinity, -0.0)]
        [InlineData(double.PositiveInfinity, 1.0)]
        public void Infinity(double? infinity, double? nonInfinity)
        {
            var nullableInfinityArg = Mocks.Domain.Guard.Guard.Infinity(Mocks.Domain.Guard.Guard.Argument(() => infinity));
            var nullableNonInfinityArg = Mocks.Domain.Guard.Guard.NotInfinity(Mocks.Domain.Guard.Guard.Argument(() => nonInfinity));
            if (!infinity.HasValue)
            {
                Mocks.Domain.Guard.Guard.NotInfinity(nullableInfinityArg);
                Mocks.Domain.Guard.Guard.Infinity(nullableNonInfinityArg);
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableNonInfinityArg,
                arg => Mocks.Domain.Guard.Guard.Infinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.Infinity(arg, d =>
                {
                    Assert.Equal(nonInfinity, d);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                nullableInfinityArg,
                arg => Mocks.Domain.Guard.Guard.NotInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotInfinity(arg, d =>
                {
                    Assert.Equal(infinity, d);
                    return message;
                }));

            var infinityArg = Mocks.Domain.Guard.Guard.Infinity(Mocks.Domain.Guard.Guard.Argument(infinity.Value, nameof(infinity)));
            var nonInfinityArg = Mocks.Domain.Guard.Guard.NotInfinity(Mocks.Domain.Guard.Guard.Argument(nonInfinity.Value, nameof(nonInfinity)));
            ThrowsArgumentOutOfRangeException(
                nonInfinityArg,
                arg => Mocks.Domain.Guard.Guard.Infinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.Infinity(arg, d =>
                {
                    Assert.Equal(nonInfinity, d);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                infinityArg,
                arg => Mocks.Domain.Guard.Guard.NotInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotInfinity(arg, d =>
                {
                    Assert.Equal(infinity, d);
                    return message;
                }));
        }

        [Theory(DisplayName = "Double: PositiveInfinity/NotPositiveInfinity")]
        [InlineData(null, null)]
        [InlineData(double.PositiveInfinity, double.NaN)]
        [InlineData(double.PositiveInfinity, double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity, -1.0)]
        [InlineData(double.PositiveInfinity, 0.0)]
        [InlineData(double.PositiveInfinity, 1.0)]
        public void PositiveInfinity(double? infinity, double? nonInfinity)
        {
            var nullableInfinityArg = Mocks.Domain.Guard.Guard.PositiveInfinity(Mocks.Domain.Guard.Guard.Argument(() => infinity));
            var nullableNonInfinityArg = Mocks.Domain.Guard.Guard.NotPositiveInfinity(Mocks.Domain.Guard.Guard.Argument(() => nonInfinity));
            if (!infinity.HasValue)
            {
                Mocks.Domain.Guard.Guard.NotPositiveInfinity(nullableInfinityArg);
                Mocks.Domain.Guard.Guard.PositiveInfinity(nullableNonInfinityArg);
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableNonInfinityArg,
                arg => Mocks.Domain.Guard.Guard.PositiveInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.PositiveInfinity(arg, d =>
                {
                    Assert.Equal(nonInfinity, d);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                nullableInfinityArg,
                arg => Mocks.Domain.Guard.Guard.NotPositiveInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotPositiveInfinity(arg, message));

            var infinityArg = Mocks.Domain.Guard.Guard.PositiveInfinity(Mocks.Domain.Guard.Guard.Argument(infinity.Value, nameof(infinity)));
            var nonInfinityArg = Mocks.Domain.Guard.Guard.NotPositiveInfinity(Mocks.Domain.Guard.Guard.Argument(nonInfinity.Value, nameof(nonInfinity)));
            ThrowsArgumentOutOfRangeException(
                nonInfinityArg,
                arg => Mocks.Domain.Guard.Guard.PositiveInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.PositiveInfinity(arg, d =>
                {
                    Assert.Equal(nonInfinity, d);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                infinityArg,
                arg => Mocks.Domain.Guard.Guard.NotPositiveInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotPositiveInfinity(arg, message));
        }

        [Theory(DisplayName = "Double: NegativeInfinity/NotNegativeInfinity")]
        [InlineData(null, null)]
        [InlineData(double.NegativeInfinity, double.NaN)]
        [InlineData(double.NegativeInfinity, -1.0)]
        [InlineData(double.NegativeInfinity, 0.0)]
        [InlineData(double.NegativeInfinity, 1.0)]
        [InlineData(double.NegativeInfinity, double.PositiveInfinity)]
        public void NegativeInfinity(double? infinity, double? nonInfinity)
        {
            var nullableInfinityArg = Mocks.Domain.Guard.Guard.NegativeInfinity(Mocks.Domain.Guard.Guard.Argument(() => infinity));
            var nullableNonInfinityArg = Mocks.Domain.Guard.Guard.NotNegativeInfinity(Mocks.Domain.Guard.Guard.Argument(() => nonInfinity));
            if (!infinity.HasValue)
            {
                Mocks.Domain.Guard.Guard.NotNegativeInfinity(nullableInfinityArg);
                Mocks.Domain.Guard.Guard.NegativeInfinity(nullableNonInfinityArg);
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableNonInfinityArg,
                arg => Mocks.Domain.Guard.Guard.NegativeInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NegativeInfinity(arg, d =>
                {
                    Assert.Equal(nonInfinity, d);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                nullableInfinityArg,
                arg => Mocks.Domain.Guard.Guard.NotNegativeInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotNegativeInfinity(arg, message));

            var infinityArg = Mocks.Domain.Guard.Guard.NegativeInfinity(Mocks.Domain.Guard.Guard.Argument(infinity.Value, nameof(infinity)));
            var nonInfinityArg = Mocks.Domain.Guard.Guard.Argument(nonInfinity.Value, nameof(nonInfinity)).NotNegativeInfinity();
            ThrowsArgumentOutOfRangeException(
                nonInfinityArg,
                arg => arg.NegativeInfinity(),
                (arg, message) => arg.NegativeInfinity(d =>
                {
                    Assert.Equal(nonInfinity, d);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                infinityArg,
                arg => arg.NotNegativeInfinity(),
                (arg, message) => arg.NotNegativeInfinity(message));
        }

        [Theory(DisplayName = "Double: Equal/NotEqual w/ delta")]
        [InlineData(null, .0, .0, .0)]
        [InlineData(.3305, .33, .3, .01)]
        [InlineData(.331, .332, .3, .01)]
        public void Equal(double? value, double equal, double nonEqual, double delta)
        {
            Test(value, nameof(value), NullableTest, NonNullableTest);

            void NullableTest(Mocks.Domain.Guard.Guard.ArgumentInfo<double?> nullableValueArg)
            {
                nullableValueArg.Equal(equal, delta).NotEqual(nonEqual, delta);
                if (!nullableValueArg.HasValue())
                {
                    nullableValueArg.Equal(nonEqual, delta).NotEqual(equal, delta);
                    return;
                }

                ThrowsArgumentOutOfRangeException(
                    nullableValueArg,
                    arg => arg.Equal(nonEqual, delta),
                    m => nullableValueArg.Secure != m.Contains(nonEqual.ToString()),
                    (arg, message) => arg.Equal(nonEqual, delta, (v, o) =>
                    {
                        Assert.Equal(value, v);
                        Assert.Equal(nonEqual, o);
                        return message;
                    }));

                ThrowsArgumentOutOfRangeException(
                    nullableValueArg,
                    arg => arg.NotEqual(equal, delta),
                    m => nullableValueArg.Secure != m.Contains(equal.ToString()),
                    (arg, message) => arg.NotEqual(equal, delta, (v, o) =>
                    {
                        Assert.Equal(value, v);
                        Assert.Equal(equal, o);
                        return message;
                    }));
            }

            void NonNullableTest(Mocks.Domain.Guard.Guard.ArgumentInfo<double> valueArg)
            {
                valueArg.Equal(equal, delta).NotEqual(nonEqual, delta);
                ThrowsArgumentOutOfRangeException(
                    valueArg,
                    arg => arg.Equal(nonEqual, delta),
                    m => valueArg.Secure != m.Contains(nonEqual.ToString()),
                    (arg, message) => arg.Equal(nonEqual, delta, (v, o) =>
                    {
                        Assert.Equal(value, v);
                        Assert.Equal(nonEqual, o);
                        return message;
                    }));

                ThrowsArgumentOutOfRangeException(
                    valueArg,
                    arg => arg.NotEqual(equal, delta),
                    m => valueArg.Secure != m.Contains(equal.ToString()),
                    (arg, message) => arg.NotEqual(equal, delta, (v, o) =>
                    {
                        Assert.Equal(value, v);
                        Assert.Equal(equal, o);
                        return message;
                    }));
            }
        }
    }
}
