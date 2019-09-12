namespace Surgical.SDK.Modules
{
    using Surgical.SDK.GUI.Menu;

    public interface IModule
    {
        #region Public Methods and Operators

        void Release();

        Menu Menu { get; }

        #endregion
    }
}