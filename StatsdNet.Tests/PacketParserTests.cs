using FluentAssertions;
using NUnit.Framework;

namespace StatsdNet.Tests
{
    internal static class ParsedPacketAssertions
    {
        public static void AssertValid(this ParsedPacket packet, string bucketName, long value, float sampleRate,
            string metricType)
        {
            packet.BucketName.Should().Be(bucketName);
            packet.Value.Should().Be(value);
            packet.SampleRate.Should().Be(sampleRate);
            packet.MetricType.Should().Be(metricType);
        }
    }

    [TestFixture]
    public class PacketParserTests
    {
        [Test]
        public void TryParse_WhenEmptyString_ReturnsFormatInvalid()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("", out outPacket);
            result.Should().Be(PacketParseResult.FormatInvalid);
            outPacket.Should().BeNull();
        }

        [Test]
        public void TryParse_WhenNullString_ReturnsFormatInvalid()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse(null, out outPacket);
            result.Should().Be(PacketParseResult.FormatInvalid);
            outPacket.Should().BeNull();
        }

        [Test]
        public void TryParse_WhenWhitespaceString_ReturnsFormatInvalid()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("    ", out outPacket);
            result.Should().Be(PacketParseResult.FormatInvalid);
            outPacket.Should().BeNull();
        }

        [Test]
        public void TryParse_WhenMissingMetricType_ReturnsFormatInvalid()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mycounter:100", out outPacket);
            result.Should().Be(PacketParseResult.FormatInvalid);
            outPacket.Should().BeNull();
        }

        [Test]
        public void TryParse_WhenValidCounterFormat_ReturnsCounterPacket()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mybucket:100|c", out outPacket);
            result.Should().Be(PacketParseResult.Ok);
            outPacket.AssertValid("mybucket", 100, 1, MetricTypeContants.Counter);
        }

        [Test]
        public void TryParse_WhenValidCounterFormatWithSample_ReturnsCounterPacket()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mybucket:100|c|@0.1", out outPacket);
            result.Should().Be(PacketParseResult.Ok);
            outPacket.AssertValid("mybucket", 100, 0.1f, MetricTypeContants.Counter);
        }

        [Test]
        public void TryParse_WhenValidSetFormat_ReturnsSetPacket()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mybucket:100|s", out outPacket);
            result.Should().Be(PacketParseResult.Ok);
            outPacket.AssertValid("mybucket", 100, 1, MetricTypeContants.Set);
        }

        [Test]
        public void TryParse_WhenValidSetFormatWithSample_ReturnsFormatInvalid()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mybucket:100|s|@0.1", out outPacket);
            result.Should().Be(PacketParseResult.FormatInvalid);
            outPacket.Should().BeNull();
        }

        [Test]
        public void TryParse_WhenInvalidCounterFormatWithSample_ReturnsFormatInvalid()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mybucket:100|c|0.1", out outPacket);
            result.Should().Be(PacketParseResult.FormatInvalid);
            outPacket.Should().BeNull();
        }

        [Test]
        public void TryParse_WhenInvalidGaugeFormatWithSample_ReturnsFormatInvalid()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mybucket:100|g|0.1", out outPacket);
            result.Should().Be(PacketParseResult.FormatInvalid);
            outPacket.Should().BeNull();
        }

        [Test]
        public void TryParse_WhenValidGaugeFormat_ReturnsGaugePacket()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mybucket:100|g", out outPacket);
            result.Should().Be(PacketParseResult.Ok);
            outPacket.AssertValid("mybucket", 100, 1, MetricTypeContants.Gauge);
        }

        [Test]
        public void TryParse_WhenGaugeFormatWithSample_ReturnsFormatInvalid()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mybucket:100|g|@0.1", out outPacket);
            result.Should().Be(PacketParseResult.FormatInvalid);
            outPacket.Should().BeNull();
        }

        [Test]
        public void TryParse_WhenValidTimerFormat_ReturnsTimerPacket()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mybucket:100|ms", out outPacket);
            result.Should().Be(PacketParseResult.Ok);
            outPacket.AssertValid("mybucket", 100, 1, MetricTypeContants.Timer);
        }

        [Test]
        public void TryParse_WhenValidTimerFormatWithSample_ReturnsTimerPacket()
        {
            ParsedPacket outPacket;
            var result = PacketParser.TryParse("mybucket:100|ms|@0.1", out outPacket);
            result.Should().Be(PacketParseResult.Ok);
            outPacket.AssertValid("mybucket", 100, 0.1f, MetricTypeContants.Timer);
        }
    }
}
