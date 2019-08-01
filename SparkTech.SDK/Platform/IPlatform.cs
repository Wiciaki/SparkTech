namespace SparkTech.SDK.Platform
{
    using SparkTech.SDK.Platform.API;

    public interface IPlatform
    {
        IObjectManager GetObjectManager();

        IRender GetRender();

        ISpellbook GetSpellbook();

        IGame GetGame();

        IEntityEvents GetEntityEvents();
    }
}