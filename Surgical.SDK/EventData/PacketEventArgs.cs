namespace Surgical.SDK.EventData
{
    using Surgical.SDK.Packets;

    public class PacketEventArgs : BlockableEventArgs
    {
        public byte[] Packet { get; }

        public PacketChannel Channel { get; }

        public PacketProtocolFlags ProtocolFlags { get; }

        public PacketEventArgs(byte[] packet, PacketChannel channel, PacketProtocolFlags protocolFlags)
        {
            this.Packet = packet;
            this.Channel = channel;
            this.ProtocolFlags = protocolFlags;
        }
    }
}