using ADDSMock.Domain.Mappings;

namespace ADDSMock.Domain.Events
{
    public class MockServicesUpdatedEvent 
    {
        private readonly IEnumerable<MockService> _services;

        public MockServicesUpdatedEvent(IEnumerable<MockService> services)
        {
            _services = services;
        }

        public IEnumerable<MockService> Services => _services;
    }
}
