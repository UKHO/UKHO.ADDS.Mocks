using System.Net.Http.Headers;
using System.Text;
using UKHO.ADDS.Mocks.Domain.Internal.Traffic;

namespace UKHO.ADDS.Mocks.Api.Models.Traffic
{
    internal static class RequestResponseModelBuilder
    {
        public static object Build(MockRequestResponse src)
        {
            var (requestIsText, requestEncoding) = Inspect(src.Request.Headers);
            var (responseIsText, responseEncoding) = Inspect(src.Response.Headers);

            if (requestIsText && responseIsText)
            {
                return new RequestResponseModel<string, string>
                {
                    Timestamp = src.Timestamp,
                    Request = new RequestModel<string>
                    {
                        Method = src.Request.Method,
                        Path = src.Request.Path,
                        QueryString = src.Request.QueryString,
                        Headers = new Dictionary<string, string>(src.Request.Headers),
                        Body = Decode(src.Request.Body, requestEncoding)
                    },
                    Response = new ResponseModel<string> { StatusCode = src.Response.StatusCode, Headers = new Dictionary<string, string>(src.Response.Headers), Body = Decode(src.Response.Body, responseEncoding) }
                };
            }

            if (requestIsText && !responseIsText)
            {
                return new RequestResponseModel<string, byte[]>
                {
                    Timestamp = src.Timestamp,
                    Request = new RequestModel<string>
                    {
                        Method = src.Request.Method,
                        Path = src.Request.Path,
                        QueryString = src.Request.QueryString,
                        Headers = new Dictionary<string, string>(src.Request.Headers),
                        Body = Decode(src.Request.Body, requestEncoding)
                    },
                    Response = new ResponseModel<byte[]> { StatusCode = src.Response.StatusCode, Headers = new Dictionary<string, string>(src.Response.Headers), Body = src.Response.Body }
                };
            }

            if (!requestIsText && responseIsText)
            {
                return new RequestResponseModel<byte[], string>
                {
                    Timestamp = src.Timestamp,
                    Request = new RequestModel<byte[]>
                    {
                        Method = src.Request.Method,
                        Path = src.Request.Path,
                        QueryString = src.Request.QueryString,
                        Headers = new Dictionary<string, string>(src.Request.Headers),
                        Body = src.Request.Body
                    },
                    Response = new ResponseModel<string> { StatusCode = src.Response.StatusCode, Headers = new Dictionary<string, string>(src.Response.Headers), Body = Decode(src.Response.Body, responseEncoding) }
                };
            }

            return new RequestResponseModel<byte[], byte[]>
            {
                Timestamp = src.Timestamp,
                Request = new RequestModel<byte[]>
                {
                    Method = src.Request.Method,
                    Path = src.Request.Path,
                    QueryString = src.Request.QueryString,
                    Headers = new Dictionary<string, string>(src.Request.Headers),
                    Body = src.Request.Body
                },
                Response = new ResponseModel<byte[]> { StatusCode = src.Response.StatusCode, Headers = new Dictionary<string, string>(src.Response.Headers), Body = src.Response.Body }
            };
        }

        private static (bool isText, Encoding encoding) Inspect(IDictionary<string, string> headers)
        {
            if (!headers.TryGetValue("Content-Type", out var raw) ||
                !MediaTypeHeaderValue.TryParse(raw, out var mt))
            {
                return (false, Encoding.UTF8);
            }

            var enc = Encoding.UTF8;
            if (mt.CharSet is not null)
            {
                try
                {
                    enc = Encoding.GetEncoding(mt.CharSet!);
                }
                catch
                {
                    enc = Encoding.UTF8;
                }
            }

            var mediaType = mt.MediaType!;
            var isText =
                mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase) ||
                mediaType.EndsWith("/json", StringComparison.OrdinalIgnoreCase) ||
                mediaType.EndsWith("+json", StringComparison.OrdinalIgnoreCase) ||
                mediaType.EndsWith("/xml", StringComparison.OrdinalIgnoreCase) ||
                mediaType.EndsWith("+xml", StringComparison.OrdinalIgnoreCase);

            return (isText, enc);
        }

        private static string Decode(byte[] data, Encoding enc)
        {
            var text = enc.GetString(data);

            // Remove BOM if present
            return text.Length > 0 && text[0] == '\uFEFF' ? text[1..] : text;
        }
    }
}
