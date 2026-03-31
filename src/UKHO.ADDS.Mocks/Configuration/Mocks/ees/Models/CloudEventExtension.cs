using System.Text.Json.Serialization;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.ees.Models
{
    public class CloudEventExtension
    {
        public string Id { get; set; } = string.Empty;

        public Uri? Source { get; set; }

        public string SpecVersion { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string DataContentType { get; set; } = string.Empty;

        public object? Data { get; set; }
    }
}
