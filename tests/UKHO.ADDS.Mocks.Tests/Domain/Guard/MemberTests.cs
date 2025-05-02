using System.Globalization;
using UKHO.ADDS.Mocks.Guard;
using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Guard
{
    public sealed class MemberTests : BaseTests
    {
        [Fact(DisplayName = "Member w/o valid expression")]
        public void InvalidMemberCall()
        {
            var dateTime = DateTime.Now;
            var dateTimeArg = Mocks.Guard.Guard.Argument(() => dateTime);

            // The final expression is not a member expression.
            Assert.Throws<ArgumentException>(
                "member",
                () => dateTimeArg.Member(dt => dt, dt => { }));

            // Expression is not composed of member expressions.
            Assert.Throws<ArgumentException>(
                "member",
                () => dateTimeArg.Member(dt => dt.ToString().Length, l => { }));

            // Member cannot be accessed.
            var accessException = new NotSupportedException();
            var obj = new TestObjectWithInaccessibleMember(accessException);
            var objArg = Mocks.Guard.Guard.Argument(() => obj);
            var memberException = Assert.Throws<ArgumentException>(
                "member",
                () => objArg.Member(o => o.InaccessibleMember, m => { }));

            Assert.Same(memberException.InnerException, accessException);
        }

        [Theory(DisplayName = "Member w/ valid expression")]
        [InlineData(null, 17, 18, false)]
        [InlineData("08/19/2018 17:42:48", 17, 18, false)]
        [InlineData("08/19/2018 17:42:48", 17, 18, true)]
        public void ValidMemberCall(string dateTimeString, int hour, int nonHour, bool secure)
        {
            var nullableDateTime = dateTimeString is null
                ? default(DateTime?)
                : DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);

            var nullableTimeArg = Mocks.Guard.Guard.Argument(() => nullableDateTime, secure)
                .Member(dt => dt.TimeOfDay.Hours, h => h.Equal(hour))
                .Member(dt => dt.TimeOfDay.Hours, h => h.Equal(hour), true);

            if (!nullableDateTime.HasValue)
            {
                nullableTimeArg
                    .Member(dt => dt.TimeOfDay.Hours, h => h.Equal(nonHour))
                    .Member(dt => dt.TimeOfDay.Hours, h => h.Equal(nonHour), true);

                return;
            }

            var dateTime = nullableDateTime.Value;

            var innerException = null as Exception;
            var thrown = ThrowsArgumentException(
                nullableTimeArg,
                arg => arg.Member(dt => dt.TimeOfDay.Hours, h => h.Equal(nonHour)),
                TestGeneratedMessage,
                (arg, message) => arg.Member(dt => dt.TimeOfDay.Hours, h => h.Equal(nonHour), (dt, h, ex) =>
                {
                    Assert.Equal(dateTime, dt);
                    Assert.Equal(hour, h);

                    TestGeneratedMessage(ex.Message);
                    innerException = ex;
                    return message;
                }));

            Assert.Same(thrown[1].InnerException, innerException);

            ThrowsArgumentOutOfRangeException(
                nullableTimeArg,
                arg => arg.Member(dt => dt.TimeOfDay.Hours, h => h.Equal(nonHour), true),
                TestGeneratedMessage,
                (arg, message) => arg.Member(dt => dt.TimeOfDay.Hours, h => h.Equal(nonHour), true, (dt, h, ex) =>
                {
                    Assert.Equal(dateTime, dt);
                    Assert.Equal(hour, h);

                    TestGeneratedMessage(ex.Message);
                    return message;
                }));

            var dateTimeArg = Mocks.Guard.Guard.Argument(() => dateTime, secure)
                .Member(dt => dt.TimeOfDay.Hours, h => h.Equal(hour))
                .Member(dt => dt.TimeOfDay.Hours, h => h.Equal(hour), true);

            thrown = ThrowsArgumentException(
                dateTimeArg,
                arg => arg.Member(dt => dt.TimeOfDay.Hours, h => h.Equal(nonHour)),
                TestGeneratedMessage,
                (arg, message) => arg.Member(dt => dt.TimeOfDay.Hours, h => h.Equal(nonHour), (dt, h, ex) =>
                {
                    Assert.Equal(dateTime, dt);
                    Assert.Equal(hour, h);

                    TestGeneratedMessage(ex.Message);
                    innerException = ex;
                    return message;
                }));

            Assert.Same(thrown[1].InnerException, innerException);

            ThrowsArgumentOutOfRangeException(
                dateTimeArg,
                arg => arg.Member(dt => dt.TimeOfDay.Hours, h => h.Equal(nonHour), true),
                TestGeneratedMessage,
                (arg, message) => arg.Member(dt => dt.TimeOfDay.Hours, h => h.Equal(nonHour), true, (dt, h, ex) =>
                {
                    Assert.Equal(dateTime, dt);
                    Assert.Equal(hour, h);

                    TestGeneratedMessage(ex.Message);
                    return message;
                }));

            bool TestGeneratedMessage(string message)
            {
                return secure != message.Contains(nonHour.ToString());
            }
        }

        private sealed class TestObjectWithInaccessibleMember
        {
            private readonly Exception accessException;

            public TestObjectWithInaccessibleMember(Exception accessException)
                => this.accessException = accessException;

            public object InaccessibleMember => throw accessException;
        }
    }
}
