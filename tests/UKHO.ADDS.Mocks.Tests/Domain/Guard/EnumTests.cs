﻿using UKHO.ADDS.Mocks.Guard;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Guard
{
    public sealed class EnumTests : BaseTests
    {
        [Flags]
        public enum Colors
        {
            None = 0,

            Red = 1,

            Green = 2,

            Blue = 4,

            All = Red | Green | Blue
        }

        [Theory(DisplayName = "Enum: Defined")]
        [InlineData(null, null)]
        [InlineData(Colors.Red, Colors.All + 1)]
        public void Defined(Colors? defined, Colors? undefined)
        {
            var nullableDefinedArg = Mocks.Guard.Guard.Argument(() => defined).Defined();
            var nullableUndefinedArg = Mocks.Guard.Guard.Argument(() => undefined);
            if (!defined.HasValue)
            {
                nullableUndefinedArg.Defined();
                return;
            }

            ThrowsArgumentException(
                nullableUndefinedArg,
                arg => arg.Defined(),
                (arg, message) => arg.Defined(c =>
                {
                    Assert.Equal(undefined, c);
                    return message;
                }));

            var definedArg = Mocks.Guard.Guard.Argument(defined.Value, nameof(defined)).Defined();
            var undefinedArg = Mocks.Guard.Guard.Argument(undefined.Value, nameof(undefined));
            ThrowsArgumentException(
                undefinedArg,
                arg => arg.Defined(),
                (arg, message) => arg.Defined(c =>
                {
                    Assert.Equal(undefined, c);
                    return message;
                }));
        }

        [Theory(DisplayName = "Enum: HasFlag/DoesNotHaveFlag")]
        [InlineData(null, Colors.All, Colors.All, false)]
        [InlineData(Colors.Red, Colors.None, Colors.Green, false)]
        [InlineData(Colors.Red, Colors.None, Colors.Green, true)]
        [InlineData(Colors.Red | Colors.Green, Colors.Red | Colors.Green, Colors.Blue, false)]
        [InlineData(Colors.Red | Colors.Green, Colors.Red | Colors.Green, Colors.Blue, true)]
        [InlineData(Colors.Red | Colors.Blue, Colors.Blue, Colors.Green, false)]
        [InlineData(Colors.Red | Colors.Blue, Colors.Blue, Colors.Green, true)]
        public void HasFlag(Colors? value, Colors setFlags, Colors unsetFlags, bool secure)
        {
            var nullableValueArg = Mocks.Guard.Guard.Argument(() => value, secure)
                .HasFlag(setFlags)
                .DoesNotHaveFlag(unsetFlags);

            if (!value.HasValue)
            {
                nullableValueArg
                    .HasFlag(unsetFlags)
                    .DoesNotHaveFlag(setFlags);

                return;
            }

            ThrowsArgumentException(
                nullableValueArg,
                arg => arg.HasFlag(unsetFlags),
                m => secure != m.Contains(unsetFlags.ToString()),
                (arg, message) => arg.HasFlag(unsetFlags, (v, f) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(unsetFlags, f);
                    return message;
                }));

            ThrowsArgumentException(
                nullableValueArg,
                arg => arg.DoesNotHaveFlag(setFlags),
                m => secure != m.Contains(setFlags.ToString()),
                (arg, message) => arg.DoesNotHaveFlag(setFlags, (v, f) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(setFlags, f);
                    return message;
                }));

            var valueArg = Mocks.Guard.Guard.Argument(value.Value, nameof(value), secure)
                .HasFlag(setFlags)
                .DoesNotHaveFlag(unsetFlags);

            ThrowsArgumentException(
                valueArg,
                arg => arg.HasFlag(unsetFlags),
                m => secure != m.Contains(unsetFlags.ToString()),
                (arg, message) => arg.HasFlag(unsetFlags, (v, f) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(unsetFlags, f);
                    return message;
                }));

            ThrowsArgumentException(
                valueArg,
                arg => arg.DoesNotHaveFlag(setFlags),
                m => secure != m.Contains(setFlags.ToString()),
                (arg, message) => arg.DoesNotHaveFlag(setFlags, (v, f) =>
                {
                    Assert.Equal(value, v);
                    Assert.Equal(setFlags, f);
                    return message;
                }));
        }
    }
}
