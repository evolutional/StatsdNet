using System;

namespace StatsdNet.Azure.EventHub.Frontend
{
    public class EventHubFrontendConfiguration
    {
        public string EventProcessorName { get; set; }
        public string EventHubName { get; set; }
        public string EventHubConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
    }
}