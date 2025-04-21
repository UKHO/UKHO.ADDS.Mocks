using ADDSMock.Models;
using Newtonsoft.Json.Linq;
using WireMock;
using WireMock.Types;
using WireMock.Util;

namespace ADDSMock.ResponseGenerator
{
    public static class FSSResponseGenerator
    {
        private static readonly string _templatePath = @"..\ADDSMock\service-configuration\fss\files\search-product.json";

        public static async Task<ResponseMessage> ProvideSearchFilterResponse(IRequestMessage requestMessage)
        {
            try
            {
                var jsonTemplate = JObject.Parse(await File.ReadAllTextAsync(_templatePath));
                var filter = requestMessage.Query["$filter"].FirstOrDefault();
                if (string.IsNullOrEmpty(filter))
                {
                    return CreateErrorResponse(400, "Missing or invalid $filter parameter", "400-badrequests-guid-fss-batch-search");

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
            CreateResponse(statusCode, new JObject
            {
                ["correlationId"] = correlationId,
                ["errors"] = new JArray
                {
                    new JObject { ["source"] = "Search Product", ["message"] = message }
                }
            }, correlationId);

        private static ResponseMessage CreateResponse(int statusCode, JObject jsonTemplate, string correlationId) =>
            new()
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, WireMockList<string>> {
                            { "Content-Type", "application/json" },
                            { "_X-Correlation-ID", correlationId }
                },
                BodyData = new BodyData
                {
                    BodyAsJson = jsonTemplate,
                    DetectedBodyType = BodyType.Json
                }
            };


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

        private static JObject CreateAttribute(string attr, object value) =>
            new()
            { ["key"] = attr, ["value"] = JToken.FromObject(value) };

        private static JArray CreateFilesArray(string productName, string batchId) =>
            new(
                CreateFileObject(productName, ".000", 874, batchId),
                CreateFileObject(productName, ".TXT", 1192, batchId)
            );

        private static JObject CreateFileObject(string productName, string extension, int fileSize, string batchId) =>
            new()
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
