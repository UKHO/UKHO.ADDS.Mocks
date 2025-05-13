using System.Diagnostics;
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

            // Prevent caching
            _group.AddEndpointFilter(async (ctx, next) =>
            {
                var context = ctx.HttpContext;

                context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
                context.Response.Headers.Pragma = "no-cache";
                context.Response.Headers.Expires = "0";
                return await next(ctx);
            });
        }

        public ServiceFragment Fragment { get; }

        public RouteHandlerBuilder MapGet(string pattern, Delegate handler)
        {
            EnsureNoMappingAndSet();

            var endpointName = GetEndpointName();

            Fragment.RecordMapping("GET", pattern, endpointName);
            return _group.MapGet(pattern, handler).WithTags(_tagName);
        }

        public RouteHandlerBuilder MapPost(string pattern, Delegate handler)
        {
            EnsureNoMappingAndSet();

            var endpointName = GetEndpointName();

            Fragment.RecordMapping("POST", pattern, endpointName);
            return _group.MapPost(pattern, handler).WithTags(_tagName);
        }

        public RouteHandlerBuilder MapPut(string pattern, Delegate handler)
        {
            EnsureNoMappingAndSet();

            var endpointName = GetEndpointName();

            Fragment.RecordMapping("PUT", pattern, endpointName);
            return _group.MapPut(pattern, handler).WithTags(_tagName);
        }

        public RouteHandlerBuilder MapPatch(string pattern, Delegate handler)
        {
            EnsureNoMappingAndSet();

            var endpointName = GetEndpointName();

            Fragment.RecordMapping("PATCH", pattern, endpointName);
            return _group.MapPatch(pattern, handler).WithTags(_tagName);
        }

        public RouteHandlerBuilder MapDelete(string pattern, Delegate handler)
        {
            EnsureNoMappingAndSet();

            var endpointName = GetEndpointName();

            Fragment.RecordMapping("DELETE", pattern, endpointName);
            return _group.MapDelete(pattern, handler).WithTags(_tagName);
        }

        public RouteHandlerBuilder MapMethods(string pattern, IEnumerable<string> httpMethods, Delegate handler)
        {
            EnsureNoMappingAndSet();
            var methods = httpMethods.ToList();

            var callerType = GetEndpointName();

            foreach (var method in methods)
            {
                Fragment.RecordMapping(method.ToUpperInvariant(), pattern, callerType);
            }

            return _group.MapMethods(pattern, methods, handler).WithTags(_tagName);
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

        private static string GetEndpointName()
        {
            var stackTrace = new StackTrace();

            for (var i = 1; i < stackTrace.FrameCount; i++)
            {
                var method = stackTrace.GetFrame(i)?.GetMethod();
                var declaringType = method?.DeclaringType;

                if (declaringType != null && declaringType != typeof(EndpointMockBuilder) && !declaringType.FullName.StartsWith("System.") && !declaringType.FullName.StartsWith("Microsoft."))
                {
                    return declaringType.Name!;
                }
            }

            throw new Exception("Could not determine endpoint name");
        }
    }
}
