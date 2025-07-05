

namespace UKHO.ADDS.Mocks.Domain.States
{
    public static class WellKnownStateHandler
    {
        public static IResult HandleWellKnownState(string state) =>
            state switch
            {
                WellKnownState.TooManyRequests => Results.StatusCode(StatusCodes.Status429TooManyRequests),
                WellKnownState.TemporaryRedirect => Results.StatusCode(StatusCodes.Status307TemporaryRedirect),
                WellKnownState.Gone => Results.StatusCode(StatusCodes.Status410Gone),
                WellKnownState.RangeNotSatisfiable => Results.StatusCode(StatusCodes.Status416RangeNotSatisfiable),
                WellKnownState.Forbidden => Results.StatusCode(StatusCodes.Status403Forbidden),
                WellKnownState.UnsupportedMediaType => Results.StatusCode(StatusCodes.Status415UnsupportedMediaType),
                WellKnownState.NotModified => Results.StatusCode(StatusCodes.Status304NotModified),
                WellKnownState.PayloadTooLarge => Results.StatusCode(StatusCodes.Status413PayloadTooLarge),
                WellKnownState.Conflict => Results.Conflict("Conflict occurred"),
                WellKnownState.NotFound => Results.NotFound("Not found"),
                WellKnownState.BadRequest => Results.BadRequest("Bad request"),
                WellKnownState.Unauthorized => Results.Unauthorized(),
                WellKnownState.InternalServerError => Results.InternalServerError(),
                WellKnownState.ImATeapot => Results.StatusCode(StatusCodes.Status418ImATeapot),
                _ => Results.NotFound("You must handle the default state in your endpoint")

            };
    }
}
