using System.Collections.ObjectModel;
using ADDSMock.Domain.Traffic;
using DynamicData.Binding;
using WireMock.Logging;

namespace ADDSMock.Domain.Services
{
    public interface ILoggingService : IWireMockLogger
    {
        ObservableCollection<RequestResponseModel> TrafficLogs { get; }
    }
}
