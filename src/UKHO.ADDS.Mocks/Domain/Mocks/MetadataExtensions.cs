using UKHO.ADDS.Mocks.Domain.Internal.Configuration;
using UKHO.ADDS.Mocks.Domain.Internal.Mocks;
using UKHO.ADDS.Mocks.Markdown;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks
{
    public static class MetadataExtensions
    {
        private static RouteHandlerBuilder WithRequiredHeader(this RouteHandlerBuilder builder, IEndpointMock mockBuilder, string name, string description)
        {
            if (mockBuilder is EndpointMockBuilder endpointBuilder)
            {
                var states = endpointBuilder.Fragment.Definition.States;

                builder.Add(x => { x.Metadata.Add(new OpenApiHeaderParameter { Name = name, Description = description, Required = false, ExpectedValues = states.Select(s => s.State) }); });
            }

            return builder;
        }

        public static RouteHandlerBuilder WithEndpointMetadata(
            this RouteHandlerBuilder builder,
            IEndpointMock mockBuilder,
            Action<IMarkdownDocument> descriptionBuilder)
        {
            builder = builder.WithRequiredHeader(mockBuilder, ServiceEndpointMock.PerRequestHeaderKey, "Set the ADDS Mock state for this request");

            var fragment = ((EndpointMockBuilder)mockBuilder).Fragment;

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

            var document = new MarkdownDocument();

            descriptionBuilder.Invoke(document);

            var (project, typePath) = ParseFullTypeName(fragment.Type.FullName!);

            if (fragment.IsOverride)
            {
                document.Append(new MarkdownHeader("[OVERRIDE]", 3));
            }

            document.Append(new MarkdownParagraph($"Project    : {project}"));
            document.Append(new MarkdownParagraph($"Definition : {typePath}"));

            document.Append(new MarkdownHeader("States", 2));

            var stateTableRows = new List<MarkdownTableRow>();

            foreach (var state in fragment.Definition.States)
            {
                var description = state.Description ?? string.Empty;
                stateTableRows.Add(new MarkdownTableRow(state.State, description));
            }

            document.Append(new MarkdownTable(new MarkdownTableHeader(new MarkdownTableHeaderCell("State"), new MarkdownTableHeaderCell("Description")), stateTableRows));

            var markdown = document.ToString();

            builder = builder.WithDescription(markdown);

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
