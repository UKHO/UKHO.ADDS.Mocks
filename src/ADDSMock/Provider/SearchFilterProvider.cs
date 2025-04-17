using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using WireMock.Types;

namespace ADDSMock.Provider
{
    public static class SearchFilterProvider
    {
        private static readonly string _templatePath = @"..\ADDSMock\service-configuration\fss\files\searchProduct.json";

        public static async Task<WireMock.ResponseMessage> ProvideSearchFilterResponse(WireMock.IRequestMessage requestMessage)
        {
            var filter = requestMessage.Query["$filter"].FirstOrDefault();

            if (string.IsNullOrEmpty(filter))
            {
                return CreateErrorResponse(400, "Filter query is missing.");
            }

            var filterDetails = ParseFilterQuery(filter);
            var jsonTemplate = JObject.Parse(await File.ReadAllTextAsync(_templatePath));

            UpdateResponseTemplate(jsonTemplate, filterDetails);

            return CreateSuccessResponse(jsonTemplate);
        }

        private static WireMock.ResponseMessage CreateErrorResponse(int statusCode, string message)
        {
            return new WireMock.ResponseMessage
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, WireMockList<string>> { { "Content-Type", "text/plain" } },
                BodyData = new WireMock.Util.BodyData
                {
                    BodyAsString = message,
                    DetectedBodyType = BodyType.String
                }
            };
        }

        private static WireMock.ResponseMessage CreateSuccessResponse(JObject jsonTemplate)
        {
            return new WireMock.ResponseMessage
            {
                StatusCode = 200,
                Headers = new Dictionary<string, WireMockList<string>> { { "Content-Type", "application/json" } },
                BodyData = new WireMock.Util.BodyData
                {
                    BodyAsJson = jsonTemplate,
                    DetectedBodyType = BodyType.Json
                }
            };
        }

        private static List<int> ExtractIntegerValues(string part)
        {
            return Regex.Matches(part.Split("eq", StringSplitOptions.TrimEntries).ElementAtOrDefault(1) ?? string.Empty, @"\d+")
                        .Select(match => int.Parse(match.Value))
                        .ToList();
        }

        private static void ParseFilterPart(string part, SearchFilterDetails batchSearchDetails, Product product)
        {
            if (part.Contains("BusinessUnit"))
            {
                batchSearchDetails.BusinessUnit = ExtractValue(part);
            }
            else if (part.Contains("$batch(ProductCode)"))
            {
                batchSearchDetails.ProductCode = ExtractValue(part);
            }
            else if (part.Contains("$batch(CellName)"))
            {
                product.ProductName = ExtractValue(part);
            }
            else if (part.Contains("$batch(UpdateNumber)"))
            {
                product.UpdateNumbers ??= new List<int>();
                product.UpdateNumbers.AddRange(part.Contains("or")
                    ? ExtractValues(part, "or").Select(int.Parse)
                    : ExtractIntegerValues(part));
            }
            else if (part.Contains("$batch(EditionNumber)"))
            {
                product.EditionNumber = int.Parse(ExtractValue(part));
            }
        }

        private static SearchFilterDetails ParseFilterQuery(string filterQuery)
        {
            var batchSearchDetails = new SearchFilterDetails { Products = new List<Product>() };
            var conditions = filterQuery.Split(")))", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var condition in conditions)
            {
                var product = new Product();
                foreach (var part in condition.Split("and", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    ParseFilterPart(part, batchSearchDetails, product);
                }
                batchSearchDetails.Products.Add(product);
            }

            return batchSearchDetails;
        }

        private static string ExtractValue(string part)
        {
            return part.Split("eq", StringSplitOptions.TrimEntries).ElementAtOrDefault(1)?.Replace("'", "") ?? string.Empty;
        }

        private static List<string> ExtractValues(string part, string delimiter)
        {
            return part.Split(delimiter, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                       .Select(ExtractValue)
                       .ToList();
        }

        private static void UpdateResponseTemplate(JObject jsonTemplate, SearchFilterDetails batchSearch)
        {
            var entries = new JArray();

            foreach (var product in batchSearch.Products)
            {
                foreach (var updateNo in product.UpdateNumbers)
                {
                    entries.Add(CreateEntry(batchSearch, product, updateNo));
                }
            }

            jsonTemplate["count"] = entries.Count;
            jsonTemplate["total"] = entries.Count;
            jsonTemplate["entries"] = entries;
            jsonTemplate["_links"] = CreateLinkObject();
        }

        private static JObject CreateEntry(SearchFilterDetails batchSearch, Product product, int updateNo)
        {
            var batchId = Guid.NewGuid();
            return new JObject
            {
                ["batchId"] = batchId,
                ["status"] = "Committed",
                ["allFilesZipSize"] = 0,
                ["attributes"] = new JArray
                    {
                        CreateAttribute("CellName", product.ProductName),
                        CreateAttribute("EditionNumber", product.EditionNumber),
                        CreateAttribute("UpdateNumber", updateNo),
                        CreateAttribute("ProductCode", batchSearch.ProductCode)
                    },
                ["businessUnit"] = batchSearch.BusinessUnit,
                ["batchPublishedDate"] = DateTime.UtcNow.AddMonths(-2),
                ["expiryDate"] = DateTime.UtcNow.AddMonths(2),
                ["isAllFilesZipAvailable"] = true,
                ["files"] = CreateFilesArray(product, batchId)
            };
        }

        private static JObject CreateAttribute(string key, object value)
        {
            return new JObject { ["key"] = JToken.FromObject(key), ["value"] = JToken.FromObject(value) };
        }

        private static JArray CreateFilesArray(Product product, Guid batchId)
        {
            return new JArray
                {
                    CreateFileObject(product.ProductName, batchId, ".000", 874),
                    CreateFileObject(product.ProductName, batchId, ".TXT", 1192)
                };
        }

        private static JObject CreateFileObject(string productName, Guid batchId, string extension, int fileSize)
        {
            return new JObject
            {
                ["filename"] = $"{productName}{extension}",
                ["fileSize"] = fileSize,
                ["mimeType"] = "text/plain",
                ["hash"] = string.Empty,
                ["links"] = new JObject
                {
                    ["get"] = new JObject
                    {
                        ["href"] = $"/batches/{batchId}/files/{productName}{extension}"
                    }
                }
            };
        }

        private static JObject CreateLinkObject()
        {
            var link = new JObject { ["href"] = "/batches?limit=10&start=0&$filter=..." };
            return new JObject
            {
                ["self"] = link,
                ["first"] = link,
                ["last"] = link
            };
        }
    }

}

