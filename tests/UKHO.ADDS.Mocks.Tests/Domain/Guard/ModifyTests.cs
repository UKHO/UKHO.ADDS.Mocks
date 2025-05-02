using UKHO.ADDS.Mocks.Guard;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Guard
{
    public sealed class ModifyTests : BaseTests
    {
        [Fact(DisplayName = "Modify: Modify using value")]
        public void ModifyUsingValue()
        {
            for (var i = 0; i < 2; i++)
            {
                var stringValue = 1.ToString();
                var stringArg = Mocks.Guard.Guard.Argument(() => stringValue, i == 1);
                Assert.False(stringArg.Modified);

                var integerValue = int.Parse(stringValue);
                var integerArg = Mocks.Guard.Guard.Modify(stringArg, integerValue);
                Assert.Equal(stringArg.Name, integerArg.Name);
                Assert.Equal(integerValue, integerArg.Value);
                Assert.Equal(stringArg.Secure, integerArg.Secure);
            }
        }

        [Fact(DisplayName = "Modify: Modify using converter")]
        public void ModifyUsingConverter()
        {
            for (var i = 0; i < 2; i++)
            {
                var stringValue = 1.ToString();
                var stringArg = Mocks.Guard.Guard.Argument(() => stringValue, i == 1);

                var integerArg = Mocks.Guard.Guard.Modify(stringArg, s => int.Parse(s));
                Assert.Equal(stringArg.Name, integerArg.Name);
                Assert.Equal(1, integerArg.Value);
                Assert.True(integerArg.Modified);
                Assert.Equal(stringArg.Secure, integerArg.Secure);

                var exception = new Exception(RandomMessage);
                Assert.Same(exception, Assert.Throws<Exception>(()
                    => Mocks.Guard.Guard.Modify<string, int>(stringArg, s => throw exception)));
            }
        }

        [Fact(DisplayName = "Modify: Wrap factory")]
        public void Wrap()
        {
            var stringValue = 1.ToString();
            for (var i = 0; i < 2; i++)
            {
                var stringArg = Mocks.Guard.Guard.Argument(() => stringValue, i == 1);
                var integerArg = Mocks.Guard.Guard.Wrap(stringArg, s => int.Parse(s));

                Assert.Equal(stringArg.Name, integerArg.Name);
                Assert.Equal(1, integerArg.Value);
                Assert.True(integerArg.Modified);
                Assert.Equal(stringArg.Secure, integerArg.Secure);

                var exception = new Exception(RandomMessage);
                Assert.DoesNotContain(exception, ThrowsArgumentException(
                    stringArg,
                    arg => Mocks.Guard.Guard.Wrap<string, int>(arg, s => throw exception),
                    (arg, message) => Mocks.Guard.Guard.Wrap<string, int>(arg, s => throw exception, s =>
                    {
                        Assert.Same(stringArg.Value, s);
                        return message;
                    })));
            }
        }

        [Fact(DisplayName = "Modify: Clone")]
        public void GuardSupportsCloning()
        {
            var cloneable = new TestCloneable();
            Assert.False(cloneable.IsClone);

            for (var i = 0; i < 2; i++)
            {
                var cloneableArg = Mocks.Guard.Guard.Argument(() => cloneable, i == 1);
                Assert.False(cloneableArg.Modified);

                var cloneArg = Mocks.Guard.Guard.Clone(cloneableArg);
                Assert.Equal(cloneableArg.Name, cloneArg.Name);
                Assert.True(cloneArg.Value.IsClone);
                Assert.Equal(cloneableArg.Modified, cloneArg.Modified);
                Assert.Equal(cloneableArg.Secure, cloneArg.Secure);

                var modifiedCloneableArg = Mocks.Guard.Guard.Modify(cloneableArg, c => new TestCloneable());
                Assert.True(modifiedCloneableArg.Modified);
                Assert.Equal(cloneableArg.Secure, modifiedCloneableArg.Secure);

                var modifiedCloneArg = modifiedCloneableArg.Clone();
                Assert.Equal(modifiedCloneableArg.Name, modifiedCloneArg.Name);
                Assert.True(modifiedCloneArg.Value.IsClone);
                Assert.Equal(modifiedCloneableArg.Modified, modifiedCloneArg.Modified);
                Assert.Equal(modifiedCloneableArg.Secure, modifiedCloneArg.Secure);
            }
        }

        private sealed class TestCloneable : ICloneable
        {
            public TestCloneable()
            {
            }

            private TestCloneable(bool isClone) => IsClone = isClone;

            public bool IsClone { get; }

            public object Clone() => new TestCloneable(true);
        }
    }
}
