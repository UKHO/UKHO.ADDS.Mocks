using System.Text.Json;
using System.Text.Json.Nodes;
using ADDSMock.Models;
using WireMock;
using WireMock.Types;
using WireMock.Util;

namespace ADDSMock.ResponseGenerator
{
    public static class FSSResponseGenerator
    {
        public static async Task<ResponseMessage> ProvideSearchFilterResponse(IRequestMessage requestMessage, string templatePath)
        {
            try
            {
                var jsonTemplate = JsonNode.Parse(await File.ReadAllTextAsync(templatePath))?.AsObject();

                var filter = requestMessage.Query["$filter"].FirstOrDefault();
                if (string.IsNullOrEmpty(filter))
                {
                    return CreateErrorResponse(400, "Missing or invalid $filter parameter.", "400-badrequest-guid-fss-batch-search");
                }

                var batchDetails = BatchQueryParser.ParseBatchQuery("$filter=" + filter);
                UpdateResponseTemplate(jsonTemplate, batchDetails);
                return CreateResponse(200, jsonTemplate, "200-ok-guid-fss-batch-search");
            }
            catch (Exception)
            {
                return CreateErrorResponse(500,
                    "Error occurred while processing Batch Search request",
                    "500-internalservererror-guid-fss-batch-search");
            }
        }

        private static ResponseMessage CreateErrorResponse(int statusCode, string message, string correlationId) =>
            CreateResponse(statusCode, new JsonObject
            {
                ["correlationId"] = correlationId,
                ["errors"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["source"] = "Search Product",
                        ["message"] = message
                    }
                }
            }, correlationId);

        private static ResponseMessage CreateResponse(int statusCode, JsonObject jsonTemplate, string correlationId) =>
            new()
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, WireMockList<string>>
                {
                    { "Content-Type", "application/json" },
                    { "_X-Correlation-ID", correlationId }
                },
                BodyData = new BodyData
                {
                    BodyAsString = JsonSerializer.Serialize(jsonTemplate, new JsonSerializerOptions { WriteIndented = true }),
                    DetectedBodyType = BodyType.String
                }
            };

        private static void UpdateResponseTemplate(JsonObject jsonTemplate, FSSSearchFilterDetails filterDetails)
        {
            var entries = new JsonArray();

            foreach (var product in filterDetails.Products)
            {
                product.UpdateNumbers?.ForEach(updateNumber =>
                {
                    var batchId = Guid.NewGuid().ToString();
                    entries.Add(new JsonObject
                    {
                        ["batchId"] = batchId,
                        ["status"] = "Committed",
                        ["allFilesZipSize"] = null,
                        ["attributes"] = new JsonArray
                        {
                            CreateAttribute("ProductName", product.ProductName),
                            CreateAttribute("EditionNumber", product.EditionNumber),
                            CreateAttribute("UpdateNumber", updateNumber),
                            CreateAttribute("ProductType", filterDetails.ProductType)
                        },
                        ["businessUnit"] = filterDetails.BusinessUnit,
                        ["batchPublishedDate"] = DateTime.UtcNow.AddMonths(-2).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        ["expiryDate"] = DateTime.UtcNow.AddMonths(2).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        ["isAllFilesZipAvailable"] = true,
                        ["files"] = CreateFilesArray(product.ProductName, batchId, updateNumber)
                    });
                });
            }

            jsonTemplate["count"] = entries.Count;
            jsonTemplate["total"] = entries.Count;
            jsonTemplate["entries"] = entries;
            jsonTemplate["_links"] = CreateLinkObject(filterDetails.ProductType, filterDetails.Products.FirstOrDefault());
        }

        private static JsonObject CreateAttribute(string attr, object value) =>
            new()
            {
                ["key"] = attr,
                ["value"] = JsonValue.Create(value)
            };

        private static JsonArray CreateFilesArray(string productName, string batchId, string updateNo) =>
            new()
            {
                CreateFileObject(productName, $".{updateNo.PadLeft(3,'0')}", 874, batchId),
                CreateFileObject(productName, ".TXT", 1192, batchId)
            };

        private static JsonObject CreateFileObject(string productName, string extension, int fileSize, string batchId) =>
            new()
            {
                ["filename"] = $"{productName}{extension}",
                ["fileSize"] = fileSize,
                ["mimeType"] = "text/plain",
                ["hash"] = string.Empty,
                ["attributes"] = new JsonArray(),
                ["links"] = new JsonObject
                {
                    ["get"] = new JsonObject { ["href"] = $"/batch/{batchId}/files/{productName}{extension}" }
                }
            };

        private static JsonObject CreateLinkObject(string productType, Product product)
        {
            var filterValue = !string.IsNullOrEmpty(product?.ProductName)
                ? $"$batch(ProductType) eq '{productType}' and $batch(ProductName) eq '{product.ProductName}' and $batch(EditionNumber) eq '{product.EditionNumber}' and $batch(UpdateNumber) eq '{product.UpdateNumbers.FirstOrDefault()}'"
                : $"$batch(ProductType) eq '{productType}'";

            var encodedFilterUrl = $"/batch?limit=10&start=0&$filter={Uri.EscapeDataString(filterValue)}";

            return new JsonObject
            {
                ["self"] = new JsonObject { ["href"] = encodedFilterUrl },
                ["first"] = new JsonObject { ["href"] = encodedFilterUrl },
                ["last"] = new JsonObject { ["href"] = encodedFilterUrl }
            };
        }
    }
}
