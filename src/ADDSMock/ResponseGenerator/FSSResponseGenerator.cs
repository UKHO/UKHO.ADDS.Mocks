using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using WireMock.Types;
using ADDSMock.Models;
using WireMock.Util;
using WireMock;

namespace ADDSMock.ResponseGenerator
{
    public static class FSSResponseGenerator
    {
        private static readonly string _templatePath = @"..\ADDSMock\service-configuration\fss\files\searchProduct.json";

        public static async Task<ResponseMessage> ProvideSearchFilterResponse(WireMock.IRequestMessage requestMessage)
        {
            var jsonTemplate = JObject.Parse(await File.ReadAllTextAsync(_templatePath));
            var filter = requestMessage.Query["$filter"].FirstOrDefault();
            var filterDetails = ParseFilterQuery(filter);
            UpdateResponseTemplate(jsonTemplate, filterDetails);
            return CreateResponse(200, jsonTemplate);
        }      

        private static ResponseMessage CreateResponse(int statusCode, JObject jsonTemplate) =>
            new()
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, WireMockList<string>> { { "Content-Type", "application/json" } },
                BodyData = new BodyData
                {
                    BodyAsJson = jsonTemplate,
                    DetectedBodyType = BodyType.Json
                }
            };

        private static FSSSearchFilterDetails ParseFilterQuery(string filterQuery)
        {
            var filterDetails = new FSSSearchFilterDetails { Products = [] };
            var conditions = filterQuery.Split(")))", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var condition in conditions)
            {
                var product = new Product();
                condition.Split("and", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                         .ToList()
                         .ForEach(part => ParseFilterPart(part, filterDetails, product));

                filterDetails.Products.Add(product);
            }

            return filterDetails;
        }

        private static void ParseFilterPart(string part, FSSSearchFilterDetails filterDetails, Product product)
        {
            switch (part)
            {
                case var _ when part.Contains("BusinessUnit"):
                    filterDetails.BusinessUnit = ExtractValue(part);
                    break;
                case var _ when part.Contains("$batch(ProductCode)"):
                    filterDetails.ProductCode = ExtractValue(part);
                    break;
                case var _ when part.Contains("$batch(CellName)"):
                    product.ProductName = ExtractValue(part);
                    break;
                case var _ when part.Contains("$batch(UpdateNumber)"):
                    product.UpdateNumbers = ExtractNumericValues(part);
                    break;
                case var _ when part.Contains("$batch(EditionNumber)"):
                    product.EditionNumber = int.Parse(ExtractValue(part));
                    break;
            }
        }

        private static string ExtractValue(string part) =>
            part.Split("eq", StringSplitOptions.TrimEntries).ElementAtOrDefault(1)?.Trim('\'') ?? string.Empty;

        private static List<int> ExtractNumericValues(string part) =>
            Regex.Matches(part, @"\d+")
                 .Select(match => int.Parse(match.Value))
                 .ToList();

        private static void UpdateResponseTemplate(JObject jsonTemplate, FSSSearchFilterDetails filterDetails)
        {
            var entries = new JArray();

            foreach (var product in filterDetails.Products)
            {
                product.UpdateNumbers?.ForEach(updateNumber =>
                {
                    var batchId = Guid.NewGuid().ToString();
                    entries.Add(new JObject
                    {
                        ["batchId"] = batchId,
                        ["status"] = "Committed",
                        ["allFilesZipSize"] = null,
                        ["attributes"] = new JArray
                        {
                            CreateAttribute("CellName", product.ProductName),
                            CreateAttribute("EditionNumber", product.EditionNumber),
                            CreateAttribute("UpdateNumber", updateNumber),
                            CreateAttribute("ProductCode", filterDetails.ProductCode)
                        },
                        ["businessUnit"] = filterDetails.BusinessUnit,
                        ["batchPublishedDate"] = DateTime.UtcNow.AddMonths(-2).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        ["expiryDate"] = DateTime.UtcNow.AddMonths(2).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        ["isAllFilesZipAvailable"] = true,
                        ["files"] = CreateFilesArray(product.ProductName, batchId)
                    });
                });
            }

            jsonTemplate["count"] = entries.Count;
            jsonTemplate["total"] = entries.Count;
            jsonTemplate["entries"] = entries;
            jsonTemplate["_links"] = CreateLinkObject(filterDetails.ProductCode, filterDetails.Products.FirstOrDefault());
        }

        private static JObject CreateAttribute(string key, object value) =>
            new JObject { ["key"] = key, ["value"] = JToken.FromObject(value) };

        private static JArray CreateFilesArray(string productName, string batchId) =>
            new JArray(
                CreateFileObject(productName, ".000", 874, batchId),
                CreateFileObject(productName, ".TXT", 1192, batchId)
            );

        private static JObject CreateFileObject(string productName, string extension, int fileSize, string batchId) =>
            new JObject
            {
                ["filename"] = $"{productName}{extension}",
                ["fileSize"] = fileSize,
                ["mimeType"] = "text/plain",
                ["hash"] = string.Empty,
                ["links"] = new JObject
                {
                    ["get"] = new JObject { ["href"] = $"/batch/{batchId}/files/{productName}{extension}" }
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
