using System;

namespace StatsdNet.Middleware.Service
{
    public class StatsdServiceConfig
    {
        public StatsdServiceConfig()
        {
            FlushInterval = TimeSpan.FromSeconds(10);
            ServiceStatsPrefix = StatsdServiceStatConstants.DefaultPrefix;
            ClearKeys = false;
            DeleteGauges = false;
        }

        public TimeSpan FlushInterval { get; set; }
        public string ServiceStatsPrefix { get; set; }
        public bool ClearKeys { get; set; }
        public bool DeleteGauges { get; set; }
    }
}