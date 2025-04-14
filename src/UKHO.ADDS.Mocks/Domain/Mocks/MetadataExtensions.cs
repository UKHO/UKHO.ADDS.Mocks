using UKHO.ADDS.Mocks.Domain.Configuration;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks
{
    public static class MetadataExtensions
    {
        public static RouteHandlerBuilder WithEndpointMetadata(
            this RouteHandlerBuilder builder,
            IServiceMockBuilder mockBuilder,
            Action<IServiceMarkdownBuilder> descriptionBuilder)
        {
            var fragment = ((ServiceMockBuilder)mockBuilder).Fragment;

            builder.Add(endpointBuilder =>
            {
                // 🔒 Ensure no conflicting metadata
                if (HasMetadata(endpointBuilder, "EndpointSummaryMetadata") ||
                    HasMetadata(endpointBuilder, "EndpointDescriptionMetadata") ||
                    HasMetadata(endpointBuilder, "EndpointNameMetadata") ||
                    HasMetadata(endpointBuilder, "EndpointTagsMetadata") ||
                    HasMetadata(endpointBuilder, "EndpointSummaryAttribute") ||
                    HasMetadata(endpointBuilder, "EndpointDescriptionAttribute"))
                {
                    throw new InvalidOperationException(
                        "This endpoint already has metadata from attributes or earlier calls (e.g., WithSummary, WithDescription, WithTags). " +
                        "Use WithEndpointMetadata() exclusively and do not mix metadata sources.");
                }
            });

            var markdownBuilder = new ServiceMarkdownBuilder();

            descriptionBuilder.Invoke(markdownBuilder);

            markdownBuilder.AppendLine();
            markdownBuilder.AppendLine();

            var (project, typePath) = ParseFullTypeName(fragment.Type.FullName!);

            if (fragment.IsOverride)
            {
                markdownBuilder.AppendLine("[OVERRIDE]");
                markdownBuilder.AppendLine();
            }

            markdownBuilder.AppendLine($"Project  : {project}");
            markdownBuilder.AppendLine();
            markdownBuilder.AppendLine($"Endpoint : {typePath}");
            markdownBuilder.AppendLine();


            builder = builder.WithDescription(markdownBuilder.ToString());

            return builder;
        }

        public static (string Project, string TypePath) ParseFullTypeName(string fullTypeName)
        {
            var parts = fullTypeName.Split('.');

            // Locate "Configuration" or "Override"
            var modeIndex = Array.FindIndex(parts, p => p is "Configuration" or "Override");

            if (modeIndex == -1 || modeIndex + 3 >= parts.Length)
            {
                throw new ArgumentException("Unexpected namespace structure", nameof(fullTypeName));
            }

            // Project: everything up to and including the service name (before mode segment)
            var project = string.Join(".", parts.Take(modeIndex));

            // Get the type name (last segment)
            var typeName = parts[^1];

            // Get the immediate folder before the type 
            var folderName = parts[^2];

            var typePath = $"{folderName}.{typeName}";

            return (project, typePath);
        }

        private static bool HasMetadata(EndpointBuilder endpointBuilder, string metadataTypeName) =>
            endpointBuilder.Metadata.Any(m => m.GetType().Name.Contains(metadataTypeName, StringComparison.InvariantCultureIgnoreCase));
    }
}
