namespace SharpLinker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.Modules;
    using Surgical.SDK.Security;

    internal class Sandbox : ISandbox
    {
        public List<IModule> LoadModules()
        {
            var folder = Folder.Root.GetFolder("ThirdParty");

            foreach (var path in Directory.EnumerateFiles(folder))
            {
                AppDomain.CurrentDomain.Load(path);
            }

            return null;
        }
    }
}