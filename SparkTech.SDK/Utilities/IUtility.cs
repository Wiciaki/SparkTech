namespace SparkTech.SDK.Utilities
{
    using GUI.Menu;

    using Modules;

    public interface IUtility : IResumable
    {
        Menu Menu { get; }
    }
}