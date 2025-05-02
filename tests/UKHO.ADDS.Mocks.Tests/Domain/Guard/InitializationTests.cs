using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Guard
{
    public sealed class InitializationTests : BaseTests
    {
        [Fact(DisplayName = "Argument: Uninitialized")]
        public void Uninitialized()
        {
            var int32Arg = default(Mocks.Guard.Guard.ArgumentInfo<int>);
            Assert.Equal(default, int32Arg.Value);
            Assert.Contains(typeof(int).ToString(), int32Arg.Name);
            Assert.False(int32Arg.Modified);
            Assert.False(int32Arg.Secure);

            var nullableInt32Arg = default(Mocks.Guard.Guard.ArgumentInfo<int?>);
            Assert.Equal(default, nullableInt32Arg.Value);
            Assert.Contains(typeof(int?).ToString(), nullableInt32Arg.Name);
            Assert.False(nullableInt32Arg.Modified);
            Assert.False(nullableInt32Arg.Secure);

            var stringArg = default(Mocks.Guard.Guard.ArgumentInfo<string>);
            Assert.Equal(default, stringArg.Value);
            Assert.Contains(typeof(string).ToString(), stringArg.Name);
            Assert.False(stringArg.Modified);
            Assert.False(stringArg.Secure);
        }

        [Theory(DisplayName = "Argument: Member expression")]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData("S")]
        public void MemberExpression<T>(T value)
        {
            for (var i = 0; i < 3; i++)
            {
                var arg = i == 0 ? Mocks.Guard.Guard.Argument(() => value)
                    : i == 1 ? Mocks.Guard.Guard.Argument(() => value)
                    : Mocks.Guard.Guard.Argument(() => value, true);

                Assert.Equal(value, arg.Value);
                Assert.Equal(nameof(value), arg.Name);
                Assert.False(arg.Modified);
                Assert.Equal(i == 2, arg.Secure);
            }
        }

        [Fact(DisplayName = "Argument: Validates member expression")]
        public void ValidatesExpression()
        {
            Assert.Throws<ArgumentNullException>("e", () => Mocks.Guard.Guard.Argument<int>(null));
            Assert.Throws<ArgumentException>("e", () => Mocks.Guard.Guard.Argument(() => 1));
        }

        [Theory(DisplayName = "Argument: Value and name")]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData("S")]
        public void ValueAndName<T>(T value)
        {
            for (var i = 0; i < 3; i++)
            {
                var arg = i == 0 ? Mocks.Guard.Guard.Argument(value, nameof(value))
                    : i == 1 ? Mocks.Guard.Guard.Argument(value, nameof(value))
                    : Mocks.Guard.Guard.Argument(value, nameof(value), true);

                Assert.Equal(value, arg.Value);
                Assert.Equal(nameof(value), arg.Name);
                Assert.False(arg.Modified);
                Assert.Equal(i == 2, arg.Secure);
            }

            for (var i = 0; i < 9; i++)
            {
                var arg = i == 0 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, nameof(value))
                    : i == 1 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, nameof(value))
                    : i == 2 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, nameof(value), true)
                    : i == 3 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, nameof(value), secure: false)
                    : i == 4 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, nameof(value))
                    : i == 5 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, nameof(value), true)
                    : i == 6 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, nameof(value), secure: true)
                    : i == 7 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, nameof(value), false, true)
                    : new Mocks.Guard.Guard.ArgumentInfo<T>(value, nameof(value), true, true);

                Assert.Equal(value, arg.Value);
                Assert.Equal(nameof(value), arg.Name);
                Assert.Equal(i == 2 || i == 5 || i == 8, arg.Modified);
                Assert.Equal(i == 6 || i == 7 || i == 8, arg.Secure);
            }
        }

        [Theory(DisplayName = "Argument: Value-only")]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData("S")]
        public void ValueOnly<T>(T value)
        {
            for (var i = 0; i < 3; i++)
            {
                var arg = i == 0 ? Mocks.Guard.Guard.Argument(value)
                    : i == 1 ? Mocks.Guard.Guard.Argument(value, secure: false)
                    : Mocks.Guard.Guard.Argument(value, secure: true);

                Assert.Equal(value, arg.Value);
                Assert.Contains(typeof(T).ToString(), arg.Name);
                Assert.False(arg.Modified);
                Assert.Equal(i == 2, arg.Secure);
            }

            for (var i = 0; i < 9; i++)
            {
                var arg = i == 0 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, null)
                    : i == 1 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, null)
                    : i == 2 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, null, true)
                    : i == 3 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, null, secure: false)
                    : i == 4 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, null)
                    : i == 5 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, null, true)
                    : i == 6 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, null, secure: true)
                    : i == 7 ? new Mocks.Guard.Guard.ArgumentInfo<T>(value, null, false, true)
                    : new Mocks.Guard.Guard.ArgumentInfo<T>(value, null, true, true);

                Assert.Equal(value, arg.Value);
                Assert.Contains(typeof(T).ToString(), arg.Name);
                Assert.Equal(i == 2 || i == 5 || i == 8, arg.Modified);
                Assert.Equal(i == 6 || i == 7 || i == 8, arg.Secure);
            }
        }

        [Theory(DisplayName = "Argument: ToString")]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData("S")]
        public void ConvertToString<T>(T value)
        {
            var valueArg = Mocks.Guard.Guard.Argument(() => value);
            if (valueArg.HasValue())
            {
                Assert.Equal(value.ToString(), valueArg.ToString());
            }
            else
            {
                Assert.Same(string.Empty, valueArg.ToString());
            }
        }

        [Theory(DisplayName = "Argument: DebuggerDisplay")]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData("S")]
        public void DebuggerDisplay<T>(T value)
        {
            const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
            for (var i = 0; i < 3; i++)
            {
                var hasName = i <= 1;
                var isSecure = i >= 1;
                var valueArg = Mocks.Guard.Guard.Argument(value, hasName ? nameof(value) : null, isSecure);

                var display = valueArg.DebuggerDisplay;
                if (hasName)
                {
                    Assert.Contains(nameof(value), display);
                }
                else
                {
                    Assert.DoesNotContain(nameof(value), display);
                }

                if (isSecure)
                {
                    Assert.Contains("SECURE", display, IgnoreCase);
                }
                else
                {
                    Assert.DoesNotContain("SECURE", display, IgnoreCase);
                }

                if (valueArg.HasValue())
                {
                    Assert.Contains(value.ToString(), display);
                }
                else
                {
                    Assert.Contains("NULL", display, IgnoreCase);
                }
            }
        }
    }
}
