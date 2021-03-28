namespace SparkTech.Ensoul.Fragments
{
    using System;

    using SDK.API;
    using SDK.EventData;
    using SDK.Packets;

    using Game = EnsoulSharp.Game;

    public class PacketFragment : IPacketFragment
    {
        public PacketFragment()
        {
            Game.OnProcessPacket += args =>
                {
                    var arg = new PacketEventArgs(args.PacketData, args.EventId, args.NetworkId);
                    this.Process(arg);

                    if (arg.IsBlocked)
                    {
                        args.Process = false;
                    }
                };
        }

        public Action<PacketEventArgs> Process { get; set; }
        public Action<PacketEventArgs> Send { get; set; }

        public void ProcessPacket(byte[] packetData, PacketChannel channel)
        {
            Console.WriteLine("Most packet stuff is not working atm");
        }

        public void SendPacket(byte[] packetData, PacketChannel channel, PacketProtocolFlags protocolFlags)
        {
            Console.WriteLine("Most packet stuff is not working atm");
        }
    }
}