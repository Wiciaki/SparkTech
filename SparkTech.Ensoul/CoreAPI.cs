namespace SparkTech.Ensoul
{
    using SparkTech.Ensoul.Fragments;

    using SDK.API;

    public class CoreAPI : ICoreAPI
    {
        public IEntityEventsFragment GetEntityEventsFragment()
        {
            return new EntityEventsFragment();
        }

        public IGameFragment GetGameFragment()
        {
            return new GameFragment();
        }

        public IObjectManagerFragment GetObjectManagerFragment()
        {
            return new ObjectManagerFragment();
        }

        public IPacketFragment GetPacketFragment()
        {
            return new PacketFragment();
        }

        public IPlayerFragment GetPlayerFragment()
        {
            return new PlayerFragment();
        }
    }
}