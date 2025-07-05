// This file is licensed under the BSD-Clause 2 license.


using System.Diagnostics;

namespace UKHO.ADDS.Mocks.Domain.Files
{
    public class UPathComparer : IComparer<UPath>, IEqualityComparer<UPath>
        , IAlternateEqualityComparer<ReadOnlySpan<char>, UPath>, IAlternateEqualityComparer<string, UPath>

    {
        public static readonly UPathComparer Ordinal = new(StringComparer.Ordinal);
        public static readonly UPathComparer OrdinalIgnoreCase = new(StringComparer.OrdinalIgnoreCase);
        public static readonly UPathComparer CurrentCulture = new(StringComparer.CurrentCulture);
        public static readonly UPathComparer CurrentCultureIgnoreCase = new(StringComparer.CurrentCultureIgnoreCase);

        private readonly StringComparer _comparer;

        private UPathComparer(StringComparer comparer)
        {
            _comparer = comparer;


            Debug.Assert(_comparer is IAlternateEqualityComparer<ReadOnlySpan<char>, string>);
        }

        bool IAlternateEqualityComparer<ReadOnlySpan<char>, UPath>.Equals(ReadOnlySpan<char> alternate, UPath other) => ((IAlternateEqualityComparer<ReadOnlySpan<char>, string>)_comparer).Equals(alternate, other.FullName);

        int IAlternateEqualityComparer<ReadOnlySpan<char>, UPath>.GetHashCode(ReadOnlySpan<char> alternate) => ((IAlternateEqualityComparer<ReadOnlySpan<char>, string>)_comparer).GetHashCode(alternate);

        UPath IAlternateEqualityComparer<ReadOnlySpan<char>, UPath>.Create(ReadOnlySpan<char> alternate) => ((IAlternateEqualityComparer<ReadOnlySpan<char>, string>)_comparer).Create(alternate);

        bool IAlternateEqualityComparer<string, UPath>.Equals(string alternate, UPath other) => _comparer.Equals(alternate, other.FullName);

        int IAlternateEqualityComparer<string, UPath>.GetHashCode(string alternate) => _comparer.GetHashCode(alternate);

        UPath IAlternateEqualityComparer<string, UPath>.Create(string alternate) => alternate;

        public int Compare(UPath x, UPath y) => _comparer.Compare(x.FullName, y.FullName);

        public bool Equals(UPath x, UPath y) => _comparer.Equals(x.FullName, y.FullName);

        public int GetHashCode(UPath obj) => _comparer.GetHashCode(obj.FullName);
    }
}
