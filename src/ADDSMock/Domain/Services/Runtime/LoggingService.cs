using System.Collections.ObjectModel;
using ADDSMock.Domain.Traffic;
using Avalonia.Threading;
using Serilog;
using WireMock.Admin.Requests;

namespace ADDSMock.Domain.Services.Runtime
{
    internal class LoggingService : ILoggingService
    {
        public virtual ObservableCollection<RequestResponseModel> TrafficLogs => [];

        public void Debug(string formatString, params object[] args) => Log.Debug(formatString, args);

        public void Info(string formatString, params object[] args)
        {
            if (!formatString.Contains("Heyenrath"))
            {
                Log.Information(formatString, args);
            }
        }

        public void Warn(string formatString, params object[] args) => Log.Warning(formatString, args);

        public void Error(string formatString, params object[] args) => Log.Error(formatString, args);

        public void Error(string message, Exception exception) => Log.Error(exception, message);

        public virtual void ProcessLogEntryModel(LogEntryModel model)
        {
        }

        public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
        {
            if (!isAdminRequest)
            {
                if (logEntryModel.Response.StatusCode != null)
                {
                    var code = (int)logEntryModel.Response.StatusCode;

                    switch (code)
                    {
                        case >=0 and < 200:
                            Log.Information($"Request [{logEntryModel.Request.Url}] with response status code [{code}]");
                            break;
                        case >= 200 and < 300:
                            Log.Information($"Request [{logEntryModel.Request.Url}] with response status code [{code}]");
                            break;
                        case >= 300 and < 400:
                            Log.Warning($"Request [{logEntryModel.Request.Url}] with response status code [{code}]");
                            break;
                        case >= 400 and < 500:
                            Log.Error($"Request [{logEntryModel.Request.Url}] with response status code [{code}]");
                            break;
                        case >= 500:
                            Log.Fatal($"Request [{logEntryModel.Request.Url}] with response status code [{code}]");
                            break;
                    }
                }
                else
                {
                    Log.Error($"Request [{logEntryModel.Request.Url}] with null response status code");
                }

                ProcessLogEntryModel(logEntryModel);
            }
        }
    }
}
