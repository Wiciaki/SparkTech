namespace SparkTech.SDK.Modules
{
    using SparkTech.SDK.UI.Menu;

    public interface IModule
    {
        #region Public Methods and Operators

        void Release();

        ModuleMenu Menu { get; }

        #endregion
    }
}