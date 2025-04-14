// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks.States
{
    public static class DefaultStateHandler
    {
        public static IResult HandleDefaultState() => HandleDefaultState(WellKnownState.Default);

        public static IResult HandleDefaultState(string state)
        {
            return state switch
            {
                WellKnownState.BadRequest => Results.BadRequest("Bad request"),
                WellKnownState.Unauthorized => Results.Unauthorized(),
                WellKnownState.Forbidden => Results.StatusCode(403),
                WellKnownState.NotFound => Results.NotFound("Not found"),
                WellKnownState.NotModified => Results.StatusCode(304),
                WellKnownState.Conflict => Results.Conflict("Conflict occurred"),
                WellKnownState.InternalServerError => Results.StatusCode(500),
                _ => Results.NotFound("State not found")
            };
        }
    }
}
