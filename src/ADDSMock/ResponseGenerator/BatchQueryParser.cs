using System.Text.RegularExpressions;
using ADDSMock.Models;

namespace ADDSMock.ResponseGenerator
{
    public class BatchQueryParser
    {
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
            var biusinessUnitPattern = @"BusinessUnit\s+eq\s+'([^']*)'";

            var businessUnitMatch = Regex.Match(filter, biusinessUnitPattern);
            filterDetails.BusinessUnit = businessUnitMatch.Success ? businessUnitMatch.Groups[1].Value : string.Empty;

            var productCodePattern = @"\$batch\(ProductCode\) eq '(?<Value>[^']*)'";
            var productCodeMatch = Regex.Match(filter, productCodePattern);
            filterDetails.ProductCode = productCodeMatch.Success ? productCodeMatch.Groups[1].Value : string.Empty;

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
            const string batchPattern = @"\$batch\((?<Property>\w+)\) eq '(?<Value>[^']*)'";
            var matches = Regex.Matches(filter, batchPattern);

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
                    case "CellName":
                        product.ProductName = value;
                        break;
                    case "EditionNumber" when int.TryParse(value, out var editionNumber):
                        product.EditionNumber = editionNumber;
                        break;
                    case "UpdateNumber" when int.TryParse(value, out var updateNumber):
                        product.UpdateNumbers.Add(updateNumber);
                        break;
                }
            }

            return product.UpdateNumbers.Count == 0 && string.IsNullOrEmpty(product.ProductName) && product.EditionNumber == 0
                ? null
                : product;
        }
    }

}
