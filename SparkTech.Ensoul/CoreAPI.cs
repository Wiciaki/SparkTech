namespace SparkTech.Ensoul
{
    using Fragments;

    using SDK.API;

    public class CoreAPI : ICoreAPI
    {
        public IEntityEventsFragment GetEntityEventsFragment() => new EntityEventsFragment();

        public IGameFragment GetGameFragment() => new GameFragment();

        public IObjectManagerFragment GetObjectManagerFragment() => new ObjectManagerFragment();

        public IPacketFragment GetPacketFragment() => new PacketFragment();

        public IPlayerFragment GetPlayerFragment() => new PlayerFragment();
    }
}