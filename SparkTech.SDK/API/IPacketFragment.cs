namespace SparkTech.SDK.API
{
    using System;

    using SparkTech.SDK.EventData;
    using SparkTech.SDK.Packets;

    public interface IPacketFragment
    {
        Action<PacketEventArgs> Process { get; set; }

        Action<PacketEventArgs> Send { get; set; }

        void ProcessPacket(byte[] packetData, PacketChannel channel);

        void SendPacket(byte[] packetData, PacketChannel channel, PacketProtocolFlags protocolFlags);
    }
}