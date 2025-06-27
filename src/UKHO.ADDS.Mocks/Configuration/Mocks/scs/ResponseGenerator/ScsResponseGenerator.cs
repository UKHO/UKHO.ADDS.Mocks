using System.Text.Json;
using System.Text.Json.Nodes;
using UKHO.ADDS.Infrastructure.Serialization.Json;

namespace UKHO.ADDS.Mocks.Configuration.Mocks.scs.ResponseGenerator
{
    public class ScsResponseGenerator
    {
        private const int MaxAdditionalUpdates = 4;

        /// <summary>
        /// Provides a mock response for product names based on the requested products.
        /// </summary>
        /// <param name="requestMessage">The HTTP request containing the product names to look up.</param>
        /// <returns>An HTTP result with the product information or an error response.</returns>
        public static async Task<IResult> ProvideProductNamesResponse(HttpRequest requestMessage)
        {
            try
            {
                // Parse and validate the request
                var validationResult = await ValidateRequestAsync(requestMessage);
                if (validationResult.errorResult != null)
                {
                    return validationResult.errorResult;
                }

                // Generate the response directly as JsonObject
                var response = GenerateProductNamesResponse(validationResult.requestedProducts);

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error processing request: {ex.Message}");
            }
        }

        private static async Task<(IResult errorResult, List<string> requestedProducts)> ValidateRequestAsync(HttpRequest request)
        {
            var requestedProducts = new List<string>();

            string requestBody;
            using (var reader = new StreamReader(request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(requestBody))
            {
                return (Results.BadRequest("Request body is required"), requestedProducts);
            }

            try
            {
                if (requestBody.TrimStart().StartsWith("["))
                {
                    var productsArray = JsonCodec.Decode<JsonElement>(requestBody);

                    if (productsArray.ValueKind != JsonValueKind.Array)
                    {
                        return (Results.BadRequest("Request body must be a JSON array of product names."), requestedProducts);
                    }

                    foreach (var product in productsArray.EnumerateArray())
                    {
                        if (product.ValueKind == JsonValueKind.String)
                        {
                            requestedProducts.Add(product.GetString());
                        }
                        else
                        {
                            return (Results.BadRequest("All items in the array must be strings."), requestedProducts);
                        }
                    }

                    return (null, requestedProducts);
                }
            }
            catch (JsonException)
            {
                return (Results.BadRequest("Invalid JSON format."), requestedProducts);
            }

            return (null, requestedProducts);
        }

        private static JsonObject GenerateProductNamesResponse(List<string> requestedProducts)
        {
            var productsArray = new JsonArray();

            foreach (var productName in requestedProducts)
            {
                productsArray.Add(GenerateProductJson(productName));
            }

            var notReturnedArray = new JsonArray();

            return new JsonObject
            {
                ["productCounts"] = new JsonObject
                {
                    ["requestedProductCount"] = requestedProducts.Count,
                    ["returnedProductCount"] = requestedProducts.Count,
                    ["requestedProductsAlreadyUpToDateCount"] = 0,
                    ["requestedProductsNotReturned"] = notReturnedArray
                },
                ["products"] = productsArray
            };
        }

        private static JsonObject GenerateProductJson(string productName)
        {
            var random = Random.Shared;
            var editionNumber = random.Next(1, 15);
            var fileSize = random.Next(2000, 15000);
            var baseDate = DateTime.UtcNow;

            var updateNumbersArray = new JsonArray { 0 };
            var datesArray = new JsonArray
            {
                new JsonObject
                {
                    ["issueDate"] = baseDate.ToString("o"),
                    ["updateApplicationDate"] = baseDate.ToString("o"), 
                    ["updateNumber"] = 0
                }
            };

            if (productName.StartsWith("101"))
            {
                var additionalUpdateCount = random.Next(0, MaxAdditionalUpdates);
                
                // Generate updates in a more functional way
                var updates = Enumerable.Range(1, 1 + additionalUpdateCount)
                    .Select(i => 
                    {
                        var currentDate = baseDate.AddDays(i * 5);
                        updateNumbersArray.Add(i);
                        return new JsonObject
                        {
                            ["issueDate"] = currentDate.ToString("o"),
                            ["updateNumber"] = i
                        };
                    }).ToList();
                
                foreach (var update in updates)
                {
                    datesArray.Add(update);
                }
            }

            var productObj = new JsonObject
            {
                ["editionNumber"] = editionNumber,
                ["productName"] = productName,
                ["updateNumbers"] = updateNumbersArray,
                ["dates"] = datesArray,
            };

            if (random.Next(0, 10) < 3)
            {
                var updateNumber = 0;
                if (updateNumbersArray.Count > 0)
                {
                    updateNumber = updateNumbersArray.Max(node => node.GetValue<int>());
                }

                productObj["cancellation"] = new JsonObject
                {
                    ["editionNumber"] = 0,
                    ["updateNumber"] = updateNumber
                };
            }

            productObj["fileSize"] = fileSize;

            return productObj;
        }
    }
}
