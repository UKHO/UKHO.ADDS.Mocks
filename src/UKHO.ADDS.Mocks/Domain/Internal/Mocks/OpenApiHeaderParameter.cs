namespace UKHO.ADDS.Mocks.Domain.Internal.Mocks
{
    internal class OpenApiHeaderParameter
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public bool Required { get; init; } = false;

        public IEnumerable<string>? ExpectedValues { get; init; }
    }
}
