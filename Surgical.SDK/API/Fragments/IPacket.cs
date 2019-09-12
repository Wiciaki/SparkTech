namespace Surgical.SDK.API.Fragments
{
    using System;

    using Surgical.SDK.EventData;
    using Surgical.SDK.Packets;

    public interface IPacket
    {
        Action<PacketEventArgs> Process { get; set; }

        Action<PacketEventArgs> Send { get; set; }

        void ProcessPacket(byte[] packetData, PacketChannel channel);

        void SendPacket(byte[] packetData, PacketChannel channel, PacketProtocolFlags protocolFlags);
    }
}