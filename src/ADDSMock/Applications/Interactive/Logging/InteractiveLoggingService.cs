using System.Collections.ObjectModel;
using ADDSMock.Domain.Services.Runtime;
using ADDSMock.Domain.Traffic;
using Avalonia.Threading;
using WireMock.Admin.Requests;

namespace ADDSMock.Applications.Interactive.Logging
{
    internal class InteractiveLoggingService : LoggingService
    {
        private readonly ObservableCollection<RequestResponseModel> _entries;
        private readonly object _lock;

        public InteractiveLoggingService()
        {
            _entries = [];
            _lock = new object();
        }

        public override ObservableCollection<RequestResponseModel> TrafficLogs => _entries;

        public override void ProcessLogEntryModel(LogEntryModel model)
        {
            lock (_lock)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    if (!model.Request.Url.Contains("favicon.ico"))
                    {
                        _entries.Add(new RequestResponseModel(model));
                    }
                });
            }
        }
    }
}
