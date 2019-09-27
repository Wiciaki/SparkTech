namespace Surgical.SDK.Modules
{
    using System;

    using Surgical.SDK.EventData;

    public interface IModulePicker<TModule> where TModule : class, IModule
    {
        #region Public Events

        event Action<BeforeValueChangeEventArgs> ModuleSelected;

        #endregion

        #region Public Properties

        TModule Current { get; }

        #endregion

        #region Public Methods and Operators

        void Add(TModule module);

        #endregion
    }
}