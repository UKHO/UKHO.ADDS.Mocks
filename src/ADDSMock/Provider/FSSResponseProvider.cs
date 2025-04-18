using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using WireMock.Types;
using ADDSMock.Models;

namespace ADDSMock.Provider
{
    public static class FSSResponseProvider
    {
        private static readonly string _templatePath = @"..\ADDSMock\service-configuration\fss\files\searchProduct.json";

        public static async Task<WireMock.ResponseMessage> ProvideSearchFilterResponse(WireMock.IRequestMessage requestMessage)
        {
            var filter = requestMessage.Query["$filter"].FirstOrDefault();

            if (string.IsNullOrEmpty(filter))
            {
                return CreateErrorResponse(400, "Missing or invalid $filter parameter");
            }

            var filterDetails = ParseFilterQuery(filter);
            var jsonTemplate = JObject.Parse(await File.ReadAllTextAsync(_templatePath));

            UpdateResponseTemplate(jsonTemplate, filterDetails);

            return CreateResponse(200, jsonTemplate);
        }

        private static WireMock.ResponseMessage CreateErrorResponse(int statusCode, string message) =>
            CreateResponse(statusCode, new JObject
            {
                ["correlationId"] = Guid.NewGuid(),
                ["errors"] = new JArray
                {
                        new JObject { ["source"] = "Search Product", ["message"] = message }
                }
            });

        private static WireMock.ResponseMessage CreateResponse(int statusCode, JObject jsonTemplate) =>
            new WireMock.ResponseMessage
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, WireMockList<string>> { { "Content-Type", "application/json" } },
                BodyData = new WireMock.Util.BodyData
                {
                    BodyAsJson = jsonTemplate,
                    DetectedBodyType = BodyType.Json
                }
            };

        private static List<int> ExtractIntegerValues(string part) =>
            Regex.Matches(part.Split("eq", StringSplitOptions.TrimEntries).ElementAtOrDefault(1) ?? string.Empty, @"\d+")
                .Select(match => int.Parse(match.Value))
                .ToList();

        private static void ParseFilterPart(string part, FSSSearchFilterDetails batchSearchDetails, Product product)
        {
            switch (part)
            {
                case var _ when part.Contains("BusinessUnit"):
                    batchSearchDetails.BusinessUnit = ExtractValue(part);
                    break;
                case var _ when part.Contains("$batch(ProductCode)"):
                    batchSearchDetails.ProductCode = ExtractValue(part);
                    break;
                case var _ when part.Contains("$batch(CellName)"):
                    product.ProductName = ExtractValue(part);
                    break;
                case var _ when part.Contains("$batch(UpdateNumber)"):
                    product.UpdateNumbers ??= [];
                    product.UpdateNumbers.AddRange(part.Contains("or")
                        ? ExtractValues(part, "or").Select(int.Parse)
                        : ExtractIntegerValues(part));
                    break;
                case var _ when part.Contains("$batch(EditionNumber)"):
                    product.EditionNumber = int.Parse(ExtractValue(part));
                    break;
            }
        }

        private static FSSSearchFilterDetails ParseFilterQuery(string filterQuery)
        {
            var fssSearchFilterDetails = new FSSSearchFilterDetails { Products = new List<Product>() };
            var conditions = filterQuery.Split(")))", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var condition in conditions)
            {
                var product = new Product();
                foreach (var part in condition.Split("and", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    ParseFilterPart(part, fssSearchFilterDetails, product);
                }
                fssSearchFilterDetails.Products.Add(product);
            }

            return fssSearchFilterDetails;
        }

        private static string ExtractValue(string part) =>
            part.Split("eq", StringSplitOptions.TrimEntries).ElementAtOrDefault(1)?.Replace("'", "") ?? string.Empty;

        private static List<string> ExtractValues(string part, string delimiter) =>
            part.Split(delimiter, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(ExtractValue)
                .ToList();

        private static void UpdateResponseTemplate(JObject jsonTemplate, FSSSearchFilterDetails fssSearchFilterDetails)
        {
            var entries = new JArray();

            foreach (var product in fssSearchFilterDetails.Products)
            {
                if (product.UpdateNumbers != null)
                {
                    foreach (var updateNo in product.UpdateNumbers)
                    {
                        entries.Add(CreateEntry(fssSearchFilterDetails, product, updateNo));
                    }
                }
            }

            jsonTemplate["count"] = entries.Count;
            jsonTemplate["total"] = entries.Count;
            jsonTemplate["entries"] = entries;
            jsonTemplate["_links"] = CreateLinkObject(fssSearchFilterDetails.ProductCode, fssSearchFilterDetails.Products.FirstOrDefault());
        }

        private static JObject CreateEntry(FSSSearchFilterDetails fssSearchFilterDetails, Product product, int updateNo) =>
            new()
            {
                ["batchId"] = Guid.NewGuid(),
                ["status"] = "Committed",
                ["allFilesZipSize"] = null,
                ["attributes"] = new JArray
                {
                        CreateAttribute("CellName", product.ProductName),
                        CreateAttribute("EditionNumber", product.EditionNumber),
                        CreateAttribute("UpdateNumber", updateNo),
                        CreateAttribute("ProductCode", fssSearchFilterDetails.ProductCode)
                },
                ["businessUnit"] = fssSearchFilterDetails.BusinessUnit,
                ["batchPublishedDate"] = DateTime.UtcNow.AddMonths(-2).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                ["expiryDate"] = DateTime.UtcNow.AddMonths(2).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                ["isAllFilesZipAvailable"] = true,
                ["files"] = CreateFilesArray(product, Guid.NewGuid())
            };

        private static JObject CreateAttribute(string key, object value) =>
            new()
            { ["key"] = JToken.FromObject(key), ["value"] = JToken.FromObject(value) };

        private static JArray CreateFilesArray(Product product, Guid batchId) =>
            [
                    CreateFileObject(product.ProductName, batchId, ".000", 874),
                    CreateFileObject(product.ProductName, batchId, ".TXT", 1192)
            ];

        private static JObject CreateFileObject(string productName, Guid batchId, string extension, int fileSize) =>
            new()
            {
                ["filename"] = $"{productName}{extension}",
                ["fileSize"] = fileSize,
                ["mimeType"] = "text/plain",
                ["hash"] = string.Empty,
                ["links"] = new JObject
                {
                    ["get"] = new JObject
                    {
                        ["href"] = $"/batch/{batchId}/files/{productName}{extension}"
                    }
                }
            };

        private static JObject CreateLinkObject(string productCode, Product product)
        {
            var filterValue = !string.IsNullOrEmpty(product?.ProductName)
                ? $"$batch(ProductCode) eq '{productCode}' and $batch(CellName) eq '{product.ProductName}' and $batch(EditionNumber) eq '{product.EditionNumber}' and $batch(UpdateNumber) eq '{product.UpdateNumbers.FirstOrDefault()}'"
                : $"$batch(ProductCode) eq '{productCode}'";

            var encodedFilterUrl = $"/batch?limit=10&start=0&$filter={Uri.EscapeDataString(filterValue)}";

            return new JObject
            {
                ["self"] = encodedFilterUrl,
                ["first"] = encodedFilterUrl,
                ["previous"] = encodedFilterUrl,
                ["next"] = encodedFilterUrl,
                ["last"] = encodedFilterUrl
            };
        }
    }
}



