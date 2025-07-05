
namespace UKHO.ADDS.Mocks.Domain
{
    public interface IEndpointMock
    {
        RouteHandlerBuilder MapGet(string pattern, Delegate handler);

        RouteHandlerBuilder MapPost(string pattern, Delegate handler);

        RouteHandlerBuilder MapPut(string pattern, Delegate handler);

        RouteHandlerBuilder MapPatch(string pattern, Delegate handler);

        RouteHandlerBuilder MapDelete(string pattern, Delegate handler);

        RouteHandlerBuilder MapMethods(string pattern, IEnumerable<string> httpMethods, Delegate handler);
    }
}
