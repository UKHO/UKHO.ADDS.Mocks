using System.Collections.Concurrent;
using UKHO.ADDS.Mocks.Domain.Internal.Logging;
using UKHO.ADDS.Mocks.Domain.Internal.Traffic;

namespace UKHO.ADDS.Mocks.Dashboard.Services
{
    internal class DashboardService
    {
        private readonly ILogger<DashboardService> _logger;
        private readonly ConcurrentQueue<MockRequestResponse> _queue;

        public event Action? Updated;

        public DashboardService(ILogger<DashboardService> logger)
        {
            _logger = logger;
            _queue = new ConcurrentQueue<MockRequestResponse>();
        }

        public void AddRequestResponse(MockRequestResponse requestResponse)
        {
            _queue.Enqueue(requestResponse);
            _logger.LogTraffic(new MockRequestResponseLogView(requestResponse));

            Updated?.Invoke();
        }

        public IReadOnlyCollection<MockRequestResponse> GetSnapshot()
        {
            return _queue.ToArray();
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }
}
