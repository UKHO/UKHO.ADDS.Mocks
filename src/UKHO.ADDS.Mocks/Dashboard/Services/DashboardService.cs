using System.Collections.ObjectModel;
using System.Text;
using UKHO.ADDS.Mocks.Domain.Internal.Traffic;

namespace UKHO.ADDS.Mocks.Dashboard.Services
{
    internal class DashboardService
    {
        public ObservableCollection<MockRequestResponse> RequestResponses { get; } = new();

        private readonly string[] methods = { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD" };
        private readonly Random random = new();

        public DashboardService()
        {
        }

        public void StartGeneratingData()
        {
            var timer = new System.Timers.Timer(3000); // 1 second
            timer.Elapsed += (_, __) =>
            {
                var item = GenerateMockItem(RequestResponses.Count + 1);
                RequestResponses.Insert(0, item); // newest at top
                if (RequestResponses.Count > 500)
                {
                    RequestResponses.RemoveAt(RequestResponses.Count - 1);
                }
            };

            timer.Start();
        }

        private MockRequestResponse GenerateMockItem(int index)
        {
            var method = methods[random.Next(methods.Length)];
            var statusCode = (index % 5) switch
            {
                0 => 200,
                1 => 302,
                2 => 404,
                3 => 500,
                _ => 418
            };
            var format = random.Next(3); // 0=json, 1=xml, 2=text

            return new MockRequestResponse
            {
                Timestamp = DateTime.Now,
                Request = new MockRequest
                {
                    Method = method,
                    Path = $"/api/mock/endpoint/{index}",
                    QueryString = $"?param={index}",
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", format == 0 ? "application/json" : format == 1 ? "application/xml" : "text/plain" },
                        { "Authorization", $"Bearer token-{index}" }
                    },
                    Body = Encoding.UTF8.GetBytes(GenerateLargeText($"REQUEST {index}", format))
                },
                Response = new MockResponse
                {
                    StatusCode = statusCode,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", format == 0 ? "application/json" : format == 1 ? "application/xml" : "text/plain" }
                    },
                    Body = Encoding.UTF8.GetBytes(GenerateLargeText($"RESPONSE {index}", format))
                }
            };
        }

        private static string GenerateLargeText(string label, int format)
        {
            var sb = new StringBuilder();

            if (format == 0) // JSON
            {
                sb.AppendLine("{");
                for (int i = 0; i < 10; i++)
                {
                    sb.AppendLine($"  \"key{i}\": \"{label} value {i}\",");
                }
                sb.AppendLine("  \"final\": \"done\"");
                sb.AppendLine("}");
            }
            else if (format == 1) // XML
            {
                sb.AppendLine($"<root label=\"{label}\">");
                for (int i = 0; i < 10; i++)
                {
                    sb.AppendLine($"  <item index=\"{i}\">{label} value {i}</item>");
                }
                sb.AppendLine("</root>");
            }
            else // Plain text
            {
                for (int i = 0; i < 50; i++)
                {
                    sb.AppendLine($"{label} LINE {i + 1}: Lorem ipsum dolor sit amet.");
                }
            }

            return sb.ToString();
        }
    }
}
