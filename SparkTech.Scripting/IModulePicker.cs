namespace SparkTech
{
    using System;

    internal interface IModulePicker<TModuleBase> where TModuleBase : class, IEntropyModule
    {
        #region Public Events

        event Action ModuleSelected;

        #endregion

        #region Public Properties

        TModuleBase Current { get; }

        string CurrentModuleName { get; }

        #endregion

        #region Public Methods and Operators

        void Add<TModule>(string moduleName) where TModule : TModuleBase, new();

        #endregion
    }
}