using UKHO.ADDS.Infrastructure.Results;
using UKHO.ADDS.Mocks.Domain.Configuration;
using UKHO.ADDS.Mocks.Domain.Internal.Logging;
using UKHO.ADDS.Mocks.Domain.Internal.Services;
using UKHO.ADDS.Mocks.Files;
using UKHO.ADDS.Mocks.States;

// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks
{
    public abstract class ServiceEndpointMock
    {
        internal const string PerRequestHeaderKey = "x-addsmockstate";
        internal const string PerEndpointHeaderKey = "x-addsmockstateendpoint";

        private ServiceDefinition? _definition;
        private ILogger<ServiceEndpointMock>? _logger;
        private FileService _fileService;

        public abstract void RegisterSingleEndpoint(IEndpointMock endpoint);

        protected string GetState(HttpRequest request)
        {
            if (_definition == null)
            {
                return WellKnownState.Default;
            }

            var endpointName = GetType().Name;
            var prefix = _definition.Prefix;

            // Check per-session (developer-set, flows)
            var sessionId = request.Headers.TryGetValue(PerEndpointHeaderKey, out var sessionHeader)
                ? sessionHeader.ToString()
                : "interactive";

            var key = $"{sessionId}/{prefix}/{endpointName}";

            if (_definition.StateOverrides.TryGetValue(key, out var perSessionValue))
            {
                _logger.LogStateSelected(new StateLogView(endpointName, prefix, sessionId, perSessionValue, StateSelectionMode.PerRequest));
                return perSessionValue;
            }

            // Check per-request (unit tests)
            if (request.Headers.TryGetValue(PerRequestHeaderKey, out var perRequestValue))
            {
                _logger.LogStateSelected(new StateLogView(endpointName, prefix, string.Empty, perRequestValue, StateSelectionMode.PerRequest));
                return perRequestValue;
            }

            _logger.LogStateSelected(new StateLogView(endpointName, prefix, string.Empty, WellKnownState.Default, StateSelectionMode.Default));
            return WellKnownState.Default;
        }

        protected void EchoHeaders(HttpRequest request, HttpResponse response, string[] headers)
        {
            foreach (var header in request.Headers
                         .Where(h => headers.Contains(h.Key, StringComparer.OrdinalIgnoreCase)))
            {
                response.Headers[header.Key] = header.Value;
            }
        }

        protected IResult<IMockFile> GetFile(string fileName)
        {
            return _fileService.GetFile(_definition!, fileName);
        }

        protected IResult<IMockFile> CreateFile(string fileName)
        {
            return _fileService.CreateFile(_definition!, fileName);
        }

        protected IResult<IMockFile> CreateFile(string fileName, Stream content)
        {
            return _fileService.CreateFile(_definition!, fileName, content);
        }

        protected IResult<IMockFile> AppendFile(string fileName, byte[] content, bool createIfNotExists = false)
        {
            return _fileService.AppendFile(_definition!, fileName, content, createIfNotExists);
        }

        protected IResult<IMockFile> AppendFile(string fileName, string content, bool createIfNotExists = false)
        {
            return _fileService.AppendFile(_definition!, fileName, content, createIfNotExists);
        }

        protected IResult<IMockFile> AppendFile(string fileName, Stream content, bool createIfNotExists = false)
        {
            return _fileService.AppendFile(_definition!, fileName, content, createIfNotExists);
        }


        internal void SetRuntime(ServiceDefinition definition, FileService fileService, ILogger<ServiceEndpointMock> logger)
        {
            _definition = definition;
            _fileService = fileService;
            _logger = logger;
        }
    }
}
