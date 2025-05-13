namespace UKHO.ADDS.Mocks.Domain.Internal.Logging
{
    internal static partial class MockLogs
    {
        private const int BaseEventId = 10000;

        private const int TrafficId = BaseEventId + 1;
        private const int StateSelectedId = BaseEventId + 2;

        public static readonly EventId Traffic = new(TrafficId, nameof(Traffic));

        [LoggerMessage(TrafficId, LogLevel.Information, "Traffic: {@requestResponse}", EventName = nameof(Traffic))]
        public static partial void LogTraffic(this ILogger logger, [LogProperties] MockRequestResponseLogView requestResponse);


        public static readonly EventId StateSelected = new(StateSelectedId, nameof(StateSelected));

        [LoggerMessage(StateSelectedId, LogLevel.Information, "State: {@state}", EventName = nameof(StateSelected))]
        public static partial void LogStateSelected(this ILogger logger, [LogProperties] StateLogView state);

    }
}
