namespace Surgical.SDK.API.Fragments
{
    using System.Collections.Generic;

    using Surgical.SDK.Modules;

    public interface ISandbox
    {
        List<IModule> LoadModules();
    }
}