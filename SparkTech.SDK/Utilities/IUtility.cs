namespace SparkTech.SDK.Utilities
{
    using SparkTech.SDK.GUI.Menu;

    public interface IUtility : IResumable
    {
        Menu Menu { get; }
    }
}