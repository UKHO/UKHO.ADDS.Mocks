using UKHO.ADDS.Infrastructure.Results;

namespace UKHO.ADDS.Mocks.Domain.Internal.Configuration
{
    internal class EndpointMockBuilder : IEndpointMock
    {
        private readonly RouteGroupBuilder _group;

        private readonly string _tagName;
        private bool _hasCreatedMapping;

        internal EndpointMockBuilder(RouteGroupBuilder group, ServiceFragment fragment)
        {
            _group = group;
            Fragment = fragment;

            _hasCreatedMapping = false;

            _tagName = GenerateTagName();
        }

        public ServiceFragment Fragment { get; }

        public RouteHandlerBuilder MapGet(string pattern, Delegate handler)
        {
            EnsureNoMappingAndSet();
            return _group.MapGet(pattern, handler).WithTags(_tagName);
        }

        public RouteHandlerBuilder MapPost(string pattern, Delegate handler)
        {
            EnsureNoMappingAndSet();
            return _group.MapPost(pattern, handler).WithTags(_tagName);
        }

        public RouteHandlerBuilder MapPut(string pattern, Delegate handler)
        {
            EnsureNoMappingAndSet();
            return _group.MapPut(pattern, handler).WithTags(_tagName);
        }

        public RouteHandlerBuilder MapPatch(string pattern, Delegate handler)
        {
            EnsureNoMappingAndSet();
            return _group.MapPatch(pattern, handler).WithTags(_tagName);
        }

        public RouteHandlerBuilder MapDelete(string pattern, Delegate handler)
        {
            EnsureNoMappingAndSet();
            return _group.MapDelete(pattern, handler).WithTags(_tagName);
        }

        public RouteHandlerBuilder MapMethods(string pattern, IEnumerable<string> httpMethods, Delegate handler)
        {
            EnsureNoMappingAndSet();
            return _group.MapMethods(pattern, httpMethods, handler).WithTags(_tagName);
        }

        public IResult<IServiceFile> GetFile(string fileName) => Fragment.GetFilePath(fileName);

        private void EnsureNoMappingAndSet()
        {
            if (_hasCreatedMapping)
            {
                throw new Exception("Only create one mapping per ServiceEndpointMock");
            }

            _hasCreatedMapping = true;
        }

        private string GenerateTagName()
        {
            var tag = $"{Fragment.ServiceName}";

            if (Fragment.IsOverride)
            {
                tag += " [OVERRIDE]";
            }

            return tag;
        }
    }
}
