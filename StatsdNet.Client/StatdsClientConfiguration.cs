namespace StatsdNet.Client
{
    public struct StatdsClientConfiguration
    {
        private static readonly StatdsClientConfiguration _default = new StatdsClientConfiguration()
        {
            MaxCoalescedPacketSize = 500
        };

        public static StatdsClientConfiguration Default { get { return _default; }}

        public int MaxCoalescedPacketSize { get; set; }
    }
}