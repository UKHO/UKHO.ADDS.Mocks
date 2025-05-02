using System.Collections.ObjectModel;
using UKHO.ADDS.Mocks.Domain.Internal.Traffic;

namespace UKHO.ADDS.Mocks.Dashboard.Services
{
    internal class DashboardService
    {
        public ObservableCollection<MockRequestResponse> RequestResponses { get; } = [];

        public void AddRequestResponse(MockRequestResponse requestResponse) => RequestResponses.Add(requestResponse);
    }
}
