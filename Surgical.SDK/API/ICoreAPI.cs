namespace Surgical.SDK.API
{
    public interface ICoreAPI
    {
        IEntityEventsFragment GetEntityEventsFragment();

        IGameFragment GetGameFragment();

        IObjectManagerFragment GetObjectManagerFragment();

        IPacketFragment GetPacketFragment();

        IPlayerFragment GetPlayerFragment();
    }
}