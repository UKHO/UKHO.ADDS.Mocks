using UKHO.ADDS.Mocks.Domain.Guard;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Guard
{
    public sealed class SingleTests : BaseTests
    {
        [Theory(DisplayName = "Single: NaN/NotNaN")]
        [InlineData(null, null)]
        [InlineData(float.NaN, float.NegativeInfinity)]
        [InlineData(float.NaN, -1.0f)]
        [InlineData(float.NaN, 0.0f)]
        [InlineData(float.NaN, 1.0f)]
        [InlineData(float.NaN, float.PositiveInfinity)]
        public void NaN(float? nan, float? nonNaN)
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
                (arg, message) => Mocks.Domain.Guard.Guard.NaN(arg, f =>
                {
                    Assert.Equal(nonNaN, f);
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
                (arg, message) => Mocks.Domain.Guard.Guard.NaN(arg, f =>
                {
                    Assert.Equal(nonNaN, f);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                nanArg,
                arg => Mocks.Domain.Guard.Guard.NotNaN(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotNaN(arg, message));
        }

        [Theory(DisplayName = "Single: Infinity/NotInfinity")]
        [InlineData(null, null)]
        [InlineData(float.NegativeInfinity, float.NaN)]
        [InlineData(float.NegativeInfinity, -1.0f)]
        [InlineData(float.NegativeInfinity, 0.0f)]
        [InlineData(float.NegativeInfinity, 1.0f)]
        [InlineData(float.PositiveInfinity, float.NaN)]
        [InlineData(float.PositiveInfinity, -1.0f)]
        [InlineData(float.PositiveInfinity, -0.0f)]
        [InlineData(float.PositiveInfinity, 1.0f)]
        public void Infinity(float? infinity, float? nonInfinity)
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
                (arg, message) => Mocks.Domain.Guard.Guard.Infinity(arg, f =>
                {
                    Assert.Equal(nonInfinity, f);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                nullableInfinityArg,
                arg => Mocks.Domain.Guard.Guard.NotInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotInfinity(arg, f =>
                {
                    Assert.Equal(infinity, f);
                    return message;
                }));

            var infinityArg = Mocks.Domain.Guard.Guard.Infinity(Mocks.Domain.Guard.Guard.Argument(infinity.Value, nameof(infinity)));
            var nonInfinityArg = Mocks.Domain.Guard.Guard.NotInfinity(Mocks.Domain.Guard.Guard.Argument(nonInfinity.Value, nameof(nonInfinity)));
            ThrowsArgumentOutOfRangeException(
                nonInfinityArg,
                arg => Mocks.Domain.Guard.Guard.Infinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.Infinity(arg, f =>
                {
                    Assert.Equal(nonInfinity, f);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                infinityArg,
                arg => Mocks.Domain.Guard.Guard.NotInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotInfinity(arg, f =>
                {
                    Assert.Equal(infinity, f);
                    return message;
                }));
        }

        [Theory(DisplayName = "Single: PositiveInfinity/NotPositiveInfinity")]
        [InlineData(null, null)]
        [InlineData(float.PositiveInfinity, float.NaN)]
        [InlineData(float.PositiveInfinity, float.NegativeInfinity)]
        [InlineData(float.PositiveInfinity, -1.0f)]
        [InlineData(float.PositiveInfinity, 0.0f)]
        [InlineData(float.PositiveInfinity, 1.0f)]
        public void PositiveInfinity(float? infinity, float? nonInfinity)
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
                (arg, message) => Mocks.Domain.Guard.Guard.PositiveInfinity(arg, f =>
                {
                    Assert.Equal(nonInfinity, f);
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
                (arg, message) => Mocks.Domain.Guard.Guard.PositiveInfinity(arg, f =>
                {
                    Assert.Equal(nonInfinity, f);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                infinityArg,
                arg => Mocks.Domain.Guard.Guard.NotPositiveInfinity(arg),
                (arg, message) => Mocks.Domain.Guard.Guard.NotPositiveInfinity(arg, message));
        }

        [Theory(DisplayName = "Single: NegativeInfinity/NotNegativeInfinity")]
        [InlineData(null, null)]
        [InlineData(float.NegativeInfinity, float.NaN)]
        [InlineData(float.NegativeInfinity, -1.0f)]
        [InlineData(float.NegativeInfinity, 0.0f)]
        [InlineData(float.NegativeInfinity, 1.0f)]
        [InlineData(float.NegativeInfinity, float.PositiveInfinity)]
        public void NegativeInfinity(float? infinity, float? nonInfinity)
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
                (arg, message) => Mocks.Domain.Guard.Guard.NegativeInfinity(arg, f =>
                {
                    Assert.Equal(nonInfinity, f);
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
                (arg, message) => arg.NegativeInfinity(f =>
                {
                    Assert.Equal(nonInfinity, f);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                infinityArg,
                arg => arg.NotNegativeInfinity(),
                (arg, message) => arg.NotNegativeInfinity(message));
        }

        [Theory(DisplayName = "Single: Equal/NotEqual w/ delta")]
        [InlineData(null, .0, .0, .0)]
        [InlineData(.3305F, .33F, .3F, .01F)]
        [InlineData(.331F, .332F, .3F, .01F)]
        public void Equal(float? value, float equal, float nonEqual, float delta)
        {
            Test(value, nameof(value), NullableTest, NonNullableTest);

            void NullableTest(Mocks.Domain.Guard.Guard.ArgumentInfo<float?> nullableValueArg)
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

            void NonNullableTest(Mocks.Domain.Guard.Guard.ArgumentInfo<float> valueArg)
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
