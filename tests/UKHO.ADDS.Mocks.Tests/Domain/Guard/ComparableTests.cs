﻿using UKHO.ADDS.Mocks.Guard;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Guard
{
    public sealed class ComparableTests : BaseTests
    {
        [Theory(DisplayName = "Comparable: Min")]
        [InlineData(null, 3, 4)]
        [InlineData(3, 3, 4)]
        [InlineData(3, 2, 5)]
        public void Min(int? value, int valueOrLess, int greaterThanValue)
        {
            var nullableValueArg = Mocks.Guard.Guard.Argument(() => value).Min(valueOrLess);
            if (!value.HasValue)
            {
                nullableValueArg.Min(greaterThanValue);
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableValueArg,
                arg => arg.Min(greaterThanValue),
                (arg, message) => arg.Min(greaterThanValue, (v, m) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(greaterThanValue, m);
                    return message;
                }));

            var valueArg = Mocks.Guard.Guard.Argument(value.Value, nameof(value)).Min(valueOrLess);
            ThrowsArgumentOutOfRangeException(
                valueArg,
                arg => arg.Min(greaterThanValue),
                (arg, message) => arg.Min(greaterThanValue, (v, m) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(greaterThanValue, m);
                    return message;
                }));
        }

        [Theory(DisplayName = "Comparable: GreaterThan")]
        [InlineData(null, 3, 4)]
        [InlineData(3, 2, 3)]
        [InlineData(3, 1, 4)]
        public void GreaterThan(int? value, int lessThanValue, int valueOrGreater)
        {
            var nullableValueArg = Mocks.Guard.Guard.Argument(() => value).GreaterThan(lessThanValue);
            if (!value.HasValue)
            {
                nullableValueArg.GreaterThan(valueOrGreater);
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableValueArg,
                arg => arg.GreaterThan(valueOrGreater),
                (arg, message) => arg.GreaterThan(valueOrGreater, (v, m) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(valueOrGreater, m);
                    return message;
                }));

            var valueArg = Mocks.Guard.Guard.Argument(value.Value, nameof(value)).GreaterThan(lessThanValue);
            ThrowsArgumentOutOfRangeException(
                valueArg,
                arg => arg.GreaterThan(valueOrGreater),
                (arg, message) => arg.GreaterThan(valueOrGreater, (v, m) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(valueOrGreater, m);
                    return message;
                }));
        }

        [Theory(DisplayName = "Comparable: Max")]
        [InlineData(null, 3, 2)]
        [InlineData(3, 3, 2)]
        [InlineData(3, 4, 1)]
        public void Max(int? value, int valueOrGreater, int lessThanValue)
        {
            var nullableValueArg = Mocks.Guard.Guard.Argument(() => value).Max(valueOrGreater);
            if (!value.HasValue)
            {
                nullableValueArg.Max(lessThanValue);
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableValueArg,
                arg => arg.Max(lessThanValue),
                (arg, message) => arg.Max(lessThanValue, (v, m) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(lessThanValue, m);
                    return message;
                }));

            var valueArg = Mocks.Guard.Guard.Argument(value.Value, nameof(value)).Max(valueOrGreater);
            ThrowsArgumentOutOfRangeException(
                valueArg,
                arg => arg.Max(lessThanValue),
                (arg, message) => arg.Max(lessThanValue, (v, m) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(lessThanValue, m);
                    return message;
                }));
        }

        [Theory(DisplayName = "Comparable: LessThan")]
        [InlineData(null, 3, 2)]
        [InlineData(3, 4, 3)]
        [InlineData(3, 5, 2)]
        public void LessThan(int? value, int greaterThanValue, int valueOrLess)
        {
            var nullableValueArg = Mocks.Guard.Guard.Argument(() => value).LessThan(greaterThanValue);
            if (!value.HasValue)
            {
                nullableValueArg.LessThan(valueOrLess);
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableValueArg,
                arg => arg.LessThan(valueOrLess),
                (arg, message) => arg.LessThan(valueOrLess, (v, m) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(valueOrLess, m);
                    return message;
                }));

            var valueArg = Mocks.Guard.Guard.Argument(value.Value, nameof(value)).LessThan(greaterThanValue);
            ThrowsArgumentOutOfRangeException(
                valueArg,
                arg => arg.LessThan(valueOrLess),
                (arg, message) => arg.LessThan(valueOrLess, (v, m) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(valueOrLess, m);
                    return message;
                }));
        }

        [Theory(DisplayName = "Comparable: InRange")]
        [InlineData(null, 2, 4)]
        [InlineData(3, 2, 4)]
        [InlineData(3, 1, 5)]
        public void InRange(int? value, int lessThanValue, int greaterThanValue)
        {
            var nullableValueArg = Mocks.Guard.Guard.Argument(() => value).InRange(lessThanValue, greaterThanValue);
            if (!value.HasValue)
            {
                for (var i = 0; i < 2; i++)
                {
                    var limit = i == 0 ? lessThanValue : greaterThanValue;
                    nullableValueArg.InRange(limit, limit);
                }

                return;
            }

            nullableValueArg
                .InRange(lessThanValue, value.Value)
                .InRange(value.Value, value.Value)
                .InRange(value.Value, greaterThanValue);

            var valueArg = Mocks.Guard.Guard.Argument(value.Value, nameof(value))
                .InRange(lessThanValue, value.Value)
                .InRange(value.Value, value.Value)
                .InRange(value.Value, greaterThanValue)
                .InRange(lessThanValue, greaterThanValue);

            for (var i = 0; i < 2; i++)
            {
                var limit = i == 0 ? lessThanValue : greaterThanValue;
                ThrowsArgumentOutOfRangeException(
                    nullableValueArg,
                    arg => arg.InRange(limit, limit),
                    (arg, message) => arg.InRange(limit, limit, (v, min, max) =>
                    {
                        Assert.Equal(value, v);
                        Assert.Equal(limit, min);
                        Assert.Equal(limit, max);
                        return message;
                    }));

                ThrowsArgumentOutOfRangeException(
                    valueArg,
                    arg => arg.InRange(limit, limit),
                    (arg, message) => arg.InRange(limit, limit, (v, min, max) =>
                    {
                        Assert.Equal(value, v);
                        Assert.Equal(limit, min);
                        Assert.Equal(limit, max);
                        return message;
                    }));
            }
        }

        [Theory(DisplayName = "Comparable: Zero/NotZero")]
        [InlineData(null, null)]
        [InlineData(0, -1)]
        [InlineData(0, 1)]
        public void Zero(int? zero, int? nonZero)
        {
            var nullableZeroArg = Mocks.Guard.Guard.Argument(zero).Zero();
            var nullableNonZeroArg = Mocks.Guard.Guard.Argument(nonZero).NotZero();
            if (!zero.HasValue)
            {
                nullableZeroArg.NotZero();
                nullableNonZeroArg.Zero();
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableNonZeroArg,
                arg => arg.Zero(),
                (arg, message) => arg.Zero(i =>
                {
                    Assert.Equal(nonZero, i);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                nullableZeroArg,
                arg => arg.NotZero(),
                (arg, message) => arg.NotZero(message));

            var zeroArg = Mocks.Guard.Guard.Argument(zero.Value, nameof(zero)).Zero();
            var nonZeroArg = Mocks.Guard.Guard.Argument(nonZero.Value, nameof(nonZero)).NotZero();
            ThrowsArgumentOutOfRangeException(
                nonZeroArg,
                arg => arg.Zero(),
                (arg, message) => arg.Zero(i =>
                {
                    Assert.Equal(nonZero, i);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                zeroArg,
                arg => arg.NotZero(),
                (arg, message) => arg.NotZero(message));
        }

        [Theory(DisplayName = "Comparable: Positive/NotPositive")]
        [InlineData(null, null)]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public void Positive(int? positive, int? nonPositive)
        {
            var nullablePositiveArg = Mocks.Guard.Guard.Argument(() => positive).Positive();
            var nullableNonPositiveArg = Mocks.Guard.Guard.Argument(() => nonPositive).NotPositive();
            if (!positive.HasValue)
            {
                nullablePositiveArg.NotPositive();
                nullableNonPositiveArg.Positive();
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableNonPositiveArg,
                arg => arg.Positive(),
                (arg, message) => arg.Positive(i =>
                {
                    Assert.Equal(nonPositive, i);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                nullablePositiveArg,
                arg => arg.NotPositive(),
                (arg, message) => arg.NotPositive(i =>
                {
                    Assert.Equal(positive, i);
                    return message;
                }));

            var positiveArg = Mocks.Guard.Guard.Argument(positive.Value, nameof(positive)).Positive();
            var nonPositiveArg = Mocks.Guard.Guard.Argument(nonPositive.Value, nameof(nonPositive)).NotPositive();
            ThrowsArgumentOutOfRangeException(
                nonPositiveArg,
                arg => arg.Positive(),
                (arg, message) => arg.Positive(i =>
                {
                    Assert.Equal(nonPositive, i);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                positiveArg,
                arg => arg.NotPositive(),
                (arg, message) => arg.NotPositive(i =>
                {
                    Assert.Equal(positive, i);
                    return message;
                }));
        }

        [Theory(DisplayName = "Comparable: Negative/NotNegative")]
        [InlineData(null, null)]
        [InlineData(-1, 0)]
        [InlineData(-1, 1)]
        public void Negative(int? negative, int? nonNegative)
        {
            var nullableNegativeArg = Mocks.Guard.Guard.Argument(() => negative).Negative();
            var nullableNonNegativeArg = Mocks.Guard.Guard.Argument(() => nonNegative).NotNegative();
            if (!negative.HasValue)
            {
                nullableNegativeArg.NotNegative();
                nullableNonNegativeArg.Negative();
                return;
            }

            ThrowsArgumentOutOfRangeException(
                nullableNonNegativeArg,
                arg => arg.Negative(),
                (arg, message) => arg.Negative(i =>
                {
                    Assert.Equal(nonNegative, i);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                nullableNegativeArg,
                arg => arg.NotNegative(),
                (arg, message) => arg.NotNegative(i =>
                {
                    Assert.Equal(negative, i);
                    return message;
                }));

            var negativeArg = Mocks.Guard.Guard.Argument(negative.Value, nameof(negative)).Negative();
            var nonNegativeArg = Mocks.Guard.Guard.Argument(nonNegative.Value, nameof(nonNegative)).NotNegative();
            ThrowsArgumentOutOfRangeException(
                nonNegativeArg,
                arg => arg.Negative(),
                (arg, message) => arg.Negative(i =>
                {
                    Assert.Equal(nonNegative, i);
                    return message;
                }));

            ThrowsArgumentOutOfRangeException(
                negativeArg,
                arg => arg.NotNegative(),
                (arg, message) => arg.NotNegative(i =>
                {
                    Assert.Equal(negative, i);
                    return message;
                }));
        }
    }
}
