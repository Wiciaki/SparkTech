namespace Surgical.SDK.Packets
{
    using System;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.EventData;

    public static class Packet
    {
        private static readonly IPacketFragment Fragment;

        static Packet()
        {
            Fragment = Platform.CoreFragment?.GetPacketFragment() ?? throw Platform.FragmentException();

            Fragment.Process = args => OnProcess.SafeInvoke(args);
            Fragment.Send = args => OnSend.SafeInvoke(args);
        }

        public static event Action<PacketEventArgs>? OnProcess;

        public static event Action<PacketEventArgs>? OnSend;

        public static void Process(byte[] packetData, PacketChannel channel)
        {
            Fragment.ProcessPacket(packetData, channel);
        }

        public static void Send(byte[] packetData, PacketChannel channel, PacketProtocolFlags protocolFlags)
        {
            Fragment.SendPacket(packetData, channel, protocolFlags);
        }
    }
}