namespace SparkTech
{
    using SparkTech.UI.Menu;

    public interface IModule
    {
        #region Public Methods and Operators

        void Release();

        ModuleMenu Menu { get; }

        #endregion
    }
}