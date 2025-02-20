using System.Text.Json;
using System.Text.Json.Serialization;
using ADDSMock.Applications.Interactive.Controls;
using Avalonia.Media;
using AvaloniaEdit.Document;
using ReactiveUI;
using WireMock.Admin.Requests;

namespace ADDSMock.Domain.Traffic
{
    public class RequestResponseModel : ReactiveObject
    {
        private readonly LogEntryModel _logEntry;
        private readonly TextDocument _requestJson;
        readonly TextDocument _responseJson;
        private static readonly JsonSerializerOptions _serializerOptions;

        static RequestResponseModel()
        {
            _serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        public RequestResponseModel(LogEntryModel logEntry)
        {
            _logEntry = logEntry;

            var requestJson = JsonSerializer.Serialize(logEntry.Request, _serializerOptions);
            var responseJson = JsonSerializer.Serialize(logEntry.Response, _serializerOptions);

            _requestJson = new TextDocument(requestJson);
            _responseJson = new TextDocument(responseJson);
        }

        public TextDocument RequestJson => _requestJson;

        public TextDocument ResponseJson => _responseJson;

        public LogEntryModel LogEntry => _logEntry;

        public IImmutableSolidColorBrush MethodTextBrush
        {
            get
            {
                return _logEntry.Request.Method.ToLowerInvariant() switch
                {
                    "get" => CommonBrushes.SelectedAlt,
                    "post" => CommonBrushes.Selected,
                    "put" => CommonBrushes.Warning,
                    "Delete" => CommonBrushes.Error,
                    _ => Brushes.White
                };
            }
        }
    }
}
