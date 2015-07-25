using System;

namespace StatsdNet
{
    public class ParsedPacket
    {
        public DateTimeOffset Timestamp { get; set; }
        public string MetricType { get; set; }
        public string BucketName { get; set; }
        public long Value { get; set; }
        public float SampleRate { get; set; }
        public bool IsModifier { get; set; }
    }
}