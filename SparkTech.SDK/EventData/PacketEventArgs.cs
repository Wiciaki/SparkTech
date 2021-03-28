namespace SparkTech.SDK.EventData
{
    public class PacketEventArgs : BlockableEventArgs
    {
        public byte[] PacketData { get; }

        public int EventId { get; }

        public int NetworkId { get; }

        public PacketEventArgs(byte[] packetData, int eventId, int networkId)
        {
            this.PacketData = packetData;
            this.EventId = eventId;
            this.NetworkId = networkId;
        }
    }
}