namespace SharpLinker
{
    using System.Collections.Generic;
    using System.Linq;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.Modules;
    using Surgical.SDK.Security;

    internal class Sandbox : ISandbox
    {
        public List<IModule> LoadModules()
        {
            var folder = Folder.Root.GetFolder("ThirdParty");

            return new IModule[0].ToList();
        }
    }
}