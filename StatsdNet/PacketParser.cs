using System.Text;

namespace StatsdNet
{
    public static class PacketParser
    {
        private static string CleanKeyName(string key)
        {
            var sb = new StringBuilder();

            foreach (var c in key)
            {
                if (c == ' ')
                {
                    sb.Append('_');
                }
                else if (c == '\t')
                {
                    sb.Append('_');
                }
                else if (c == '/')
                {
                    sb.Append('-');
                }
                else if (char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parses packet in the format [bucketName]:[value]|[metricType][[|@sampleRate]]
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="parsedPacket"></param>
        /// <returns></returns>
        public static PacketParseResult TryParse(string packet, out ParsedPacket parsedPacket)
        {
            parsedPacket = null;

            if (string.IsNullOrWhiteSpace(packet))
            {
                return PacketParseResult.FormatInvalid;
            }

            var fields = packet.Split('|');

            if (fields.Length <= 1 || fields.Length > 3)
            {
                return PacketParseResult.FormatInvalid;
            }
            
            var bucketEnd = fields[0].IndexOf(':');
            var valueStart = bucketEnd + 1;
            var valueEnd = fields[0].Length;

            if (bucketEnd < 0)
            {
                return PacketParseResult.FormatInvalid;
            }

            var bucket = CleanKeyName(fields[0].Substring(0, bucketEnd));
            var rawValue = packet.Substring(valueStart, valueEnd - valueStart);

            long value = 0;

            if (!long.TryParse(rawValue, out value))
            {
                return PacketParseResult.FormatInvalid;
            }

            var metricType = fields[1];
            float sampleRate = 1;
            bool isModifier = false;

            if (fields.Length == 3)
            {
                if (metricType == MetricTypeContants.Set || metricType == MetricTypeContants.Gauge)
                {
                    return PacketParseResult.FormatInvalid;
                }

                var sampleRateStr = fields[2];

                if (sampleRateStr[0] != '@')
                {
                    return PacketParseResult.FormatInvalid;
                }

                sampleRateStr = sampleRateStr.Substring(1, sampleRateStr.Length - 1);

                if (!float.TryParse(sampleRateStr, out sampleRate))
                {
                    return PacketParseResult.FormatInvalid;
                }
            }
            
            switch (metricType)
            {
                case MetricTypeContants.Gauge:
                {
                    if (rawValue.StartsWith("+") || rawValue.StartsWith("-"))
                    {
                        isModifier = true;
                    }
                    break;
                }
                case MetricTypeContants.Counter:
                case MetricTypeContants.Set:
                case MetricTypeContants.Timer:
                {
                    break;
                }
                default:
                {
                    return PacketParseResult.InvalidMetricType;
                }
            }

     

            parsedPacket = new ParsedPacket
            {
                BucketName = bucket,
                Value = value,
                SampleRate = sampleRate,
                MetricType = metricType,
                IsModifier = isModifier
            };

            return PacketParseResult.Ok;
        }
    }
}