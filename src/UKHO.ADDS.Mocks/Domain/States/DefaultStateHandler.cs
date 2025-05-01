// ReSharper disable once CheckNamespace
namespace UKHO.ADDS.Mocks.States
{
    public static class WellKnownStateHandler
    {
        public static IResult HandleWellKnownState(string state)
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
                _ => Results.NotFound("You must handle the default state in your endpoint")
            };
        }
    }
}
