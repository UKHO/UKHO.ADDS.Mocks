using System.Text.RegularExpressions;
using ADDSMock.Models;

namespace ADDSMock.ResponseGenerator
{
    public class BatchQueryParser
    {
        private static readonly Regex BusinessUnitRegex = new Regex(@"BusinessUnit\s*eq\s*'([^']*)'", RegexOptions.Compiled);
        private static readonly Regex ProductTypeRegex = new Regex(@"\$batch\(ProductType\) eq '(?<Value>[^']*)'", RegexOptions.Compiled);
        private const string BatchPattern = @"\$batch\((?<Property>\w+)\) eq '(?<Value>[^']*)'";

        public static FSSSearchFilterDetails ParseBatchQuery(string odataQuery)
        {
            var filterDetails = new FSSSearchFilterDetails
            {
                Products = []
            };
            var filterMatch = Regex.Match(odataQuery, @"\$filter=(.*)");
            if (!filterMatch.Success)
            {
                return filterDetails;
            }

            var filter = filterMatch.Groups[1].Value;
            ParseFilterExpression(filter, filterDetails);

            return filterDetails;
        }

        private static void ParseFilterExpression(string filter, FSSSearchFilterDetails filterDetails)
        {
            var businessUnitMatch = BusinessUnitRegex.Match(filter);
            filterDetails.BusinessUnit = businessUnitMatch.Success ? businessUnitMatch.Groups[1].Value : string.Empty;

            var productTypeMatch = ProductTypeRegex.Match(filter);
            filterDetails.ProductType = productTypeMatch.Success ? productTypeMatch.Groups[1].Value : string.Empty;

            var conditions = filter.Split(")))", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var condition in conditions)
            {
                var product = ParseFilterProductProperties(condition);
                if (product != null)
                {
                    filterDetails.Products.Add(product);
                }
            }
        }

        private static Product ParseFilterProductProperties(string filter)
        {
            var matches = Regex.Matches(filter, BatchPattern);

            if (matches.Count == 0) return null;

            var product = new Product
            {
                UpdateNumbers = []
            };

            foreach (Match match in matches)
            {
                var property = match.Groups["Property"].Value;
                var value = match.Groups["Value"].Value;

                if (string.IsNullOrEmpty(property) || string.IsNullOrEmpty(value)) continue;

                switch (property)
                {
                    case "ProductName":
                        product.ProductName = value;
                        break;
                    case "EditionNumber":
                        product.EditionNumber = value;
                        break;
                    case "UpdateNumber":
                        product.UpdateNumbers.Add(value);
                        break;
                }
            }

            return product.UpdateNumbers.Count == 0 && string.IsNullOrEmpty(product.ProductName) && string.IsNullOrEmpty(product.EditionNumber)
                ? null
                : product;
        }
    }
}
