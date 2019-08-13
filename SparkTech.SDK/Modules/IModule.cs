namespace SparkTech.SDK.Modules
{
    public interface IModule
    {
        #region Public Methods and Operators

        void Release();

        ModuleMenu Menu { get; }

        #endregion
    }
}