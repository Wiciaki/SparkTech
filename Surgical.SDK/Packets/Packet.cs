namespace Surgical.SDK.Packets
{
    using System;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.EventData;

    public static class Packet
    {
        private static IPacketFragment packet;

        internal static void Initialize(IPacketFragment fragment)
        {
            packet = fragment;

            fragment.Process = args => OnProcess.SafeInvoke(args);
            fragment.Send = args => OnSend.SafeInvoke(args);
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