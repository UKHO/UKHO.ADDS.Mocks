using WireMock.Admin.Mappings;

namespace ADDSMock.Domain.Mappings
{
    internal class MappingModelComparer : IEqualityComparer<MappingModel>
    {
        public bool Equals(MappingModel? x, MappingModel? y) => x!.Guid == y!.Guid;

        public int GetHashCode(MappingModel obj)
        {
            unchecked
            {
                if (obj == null)
                {
                    return 0;
                }

                var hashCode = obj.Guid.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Guid.GetHashCode();

                return hashCode;
            }
        }
    }
}
