using System.Collections.ObjectModel;
using UKHO.ADDS.Mocks.Domain.Internal.Logging;
using UKHO.ADDS.Mocks.Domain.Internal.Traffic;

namespace UKHO.ADDS.Mocks.Dashboard.Services
{
    internal class DashboardService
    {
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(ILogger<DashboardService> logger) => _logger = logger;

        public ObservableCollection<MockRequestResponse> RequestResponses { get; } = [];

        public void AddRequestResponse(MockRequestResponse requestResponse)
        {
            RequestResponses.Add(requestResponse);
            _logger.LogTraffic(new MockRequestResponseLogView(requestResponse));
        }
    }
}
