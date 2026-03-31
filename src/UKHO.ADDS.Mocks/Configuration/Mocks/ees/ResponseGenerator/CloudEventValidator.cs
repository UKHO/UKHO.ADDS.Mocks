using UKHO.ADDS.Mocks.Configuration.Mocks.ees.Models;

namespace UKHO.ADDS.Mocks.EES.Configuration.Mocks.ees.ResponseGenerator
{
    public static class CloudEventValidator
    {
        public static bool ValidateCloudEvent (CloudEventExtension model)
        {
            if (model == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                return false;
            }

            if (model.Source == null)
            {
                return false;
            }

            if (!ValidateSpecVersion(model.SpecVersion))
            {
                return false;
            }

            if (!ValidateType(model.Type))
            {
                return false;
            }

            if (!ValidateDataContentType(model.DataContentType))
            {
                return false;
            }

            if (model.Data == null)
            {
                return false;
            }

            return true;
        }

        private static bool ValidateSpecVersion(string? specVersion)
        {
            return string.Equals(specVersion, "1.0", StringComparison.Ordinal);
        }

        private static bool ValidateType(string? type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return false;
            }

            return SchemaStore.AllowedSchemas.Contains(type, StringComparer.OrdinalIgnoreCase);
        }
        public static bool ValidateCloudEventContents(string? eventName, object? data)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return false;
            }

            if (data == null)
            {
                return false;
            }

            return SchemaStore.AllowedSchemas.Contains(eventName, StringComparer.OrdinalIgnoreCase);
        }
        private static bool ValidateDataContentType(string? dataContentType)
        {
            return string.Equals(dataContentType, "application/json", StringComparison.OrdinalIgnoreCase);
        }
    }
}
