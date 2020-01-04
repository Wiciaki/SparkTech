namespace Surgical.SDK.API
{
    using System;

    using Surgical.SDK.EventData;
    using Surgical.SDK.Packets;

    public interface IPacketFragment
    {
        Action<PacketEventArgs> Process { get; set; }

        Action<PacketEventArgs> Send { get; set; }

        void ProcessPacket(byte[] packetData, PacketChannel channel);

        void SendPacket(byte[] packetData, PacketChannel channel, PacketProtocolFlags protocolFlags);
    }
}