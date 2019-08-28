namespace SparkTech.SDK.Platform
{
    using SparkTech.SDK.Platform.API;

    public interface IPlatform
    {
        IObjectManager GetObjectManager();

        IRender GetRender();

        ISpellbook GetSpellbook();

        IGameInterface GetGame();

        IEntityEvents GetEntityEvents();
    }
}