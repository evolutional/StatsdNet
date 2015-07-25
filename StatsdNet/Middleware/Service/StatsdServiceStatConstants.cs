namespace StatsdNet.Middleware.Service
{
    public static class StatsdServiceStatConstants
    {
        public const string DefaultPrefix = "statsd";
        public const string PacketBadFormat = "{0}.packets.bad";
        public const string PacketCountFormat = "{0}.packets.count";
        public const string MetricsCountFormat = "{0}.metrics.count";
 
        public const string ServiceUptimeFormat = "{0}.service.uptime";
    }
}