// ReSharper disable once CheckNamespace

namespace UKHO.ADDS.Mocks
{
    public interface IServiceMockBuilder
    {
        RouteHandlerBuilder MapGet(string pattern, Delegate handler);

        RouteHandlerBuilder MapPost(string pattern, Delegate handler);

        RouteHandlerBuilder MapPut(string pattern, Delegate handler);

        RouteHandlerBuilder MapPatch(string pattern, Delegate handler);

        RouteHandlerBuilder MapDelete(string pattern, Delegate handler);

        RouteHandlerBuilder MapMethods(string pattern, IEnumerable<string> httpMethods, Delegate handler);
    }
}
