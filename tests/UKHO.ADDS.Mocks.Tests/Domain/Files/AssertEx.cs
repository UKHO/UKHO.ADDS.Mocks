﻿using Xunit;

namespace UKHO.ADDS.Mocks.Tests.Domain.Files
{
    internal static class AssertEx
    {
        public static void Equivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual)
            where T : IComparable<T> =>
            Assert.Equal(expected.OrderBy(i => i), actual.OrderBy(i => i));
    }
}
