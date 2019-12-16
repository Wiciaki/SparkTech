namespace Surgical.SDK.API
{
    using Surgical.SDK.API.Fragments;

    public interface ICoreAPI
    {
        IEntityEventsFragment GetEntityEventsFragment();

        IGameFragment GetGameFragment();

        IObjectManagerFragment GetObjectManagerFragment();

        IPacketFragment GetPacketFragment();

        IPlayerFragment GetPlayerFragment();
    }
}