namespace SparkTech.SDK.Modules
{
    using SparkTech.SDK.GUI.Menu;

    public interface IModule
    {
        #region Public Methods and Operators

        void Release();

        Menu Menu { get; }

        #endregion
    }
}