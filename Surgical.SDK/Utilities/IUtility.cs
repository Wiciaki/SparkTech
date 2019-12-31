namespace Surgical.SDK.Utilities
{
    using Surgical.SDK.GUI.Menu;

    public interface IUtility : IResumable
    {
        Menu Menu { get; }
    }
}