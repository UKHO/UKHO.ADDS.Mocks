namespace ADDSMock.Constants

{
    public class MockConstants
    {
        // Headers
        public const string CorrelationIdHeader = "X-Correlation-ID";
        public const string ContentTypeHeader = "Content-Type";
        public const string ApplicationJson = "application/json";

        // Correlation ID values
        public const string CreatedCorrelationId = "201-created-guid-";
        public const string NoContentCorrelationId = "204-no-content-guid-";
        public const string NotModifiedCorrelationId = "304-notmodified-guid-";
        public const string InternalServerErrorCorrelationId = "500-internalserver-guid-";
        public const string BadRequestCorrelationId = "400-badrequest-guid-";
        public const string UnauthorizedCorrelationId = "401-unauthorized-guid-";
        public const string ForbiddenCorrelationId = "403-forbidden-guid-";
        public const string FileNotFoundCorrelationId = "404-filenotfound-guid-";
        public const string GoneCorrelationId = "410-gone-guid-";
        public const string RangeNotSatisfiableCorrelationId = "416-rangenotsatisfiable-guid-";
        public const string UnsupportedMediaTypeCorrelationId = "415-unsupportedmediatype-guid-";
        public const string TooManyRequestsCorrelationId = "429-toomanyrequests-guid-";
        public const string TemporaryRedirectCorrelationId = "307-temporaryredirect-guid-";
        public const string PayloadTooLargeCorrelationId = "413-payloadtoolarge-guid-";

        // Additional headers (optional, based on usage)
        public const string AcceptHeader = "Accept";
        public const string AuthorizationHeader = "Authorization";
    }
}
