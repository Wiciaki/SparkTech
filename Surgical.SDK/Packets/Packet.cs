namespace Surgical.SDK.Packets
{
    using System;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.EventData;

    public static class Packet
    {
        private static IPacket packet;

        internal static void Initialize(IPacket p)
        {
            packet = p;

            p.Process = args => OnProcess.SafeInvoke(args);
            p.Send = args => OnSend.SafeInvoke(args);
        }

        public static event Action<PacketEventArgs> OnProcess;

        public static event Action<PacketEventArgs> OnSend;

        public static void Process(byte[] packetData, PacketChannel channel)
        {
            packet.ProcessPacket(packetData, channel);
        }

        public static void Send(byte[] packetData, PacketChannel channel, PacketProtocolFlags protocolFlags)
        {
            packet.SendPacket(packetData, channel, protocolFlags);
        }
    }
}