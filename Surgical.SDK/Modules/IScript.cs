namespace Surgical.SDK.Modules
{
    using System.Collections.Generic;

    public interface IScript
    {
        IEnumerable<IModule> GetModules();
    }
}