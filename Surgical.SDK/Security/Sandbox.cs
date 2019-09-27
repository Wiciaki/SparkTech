namespace Surgical.SDK.Security
{
    using System;
    using System.IO;
    using System.Linq;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.Logging;

    public static class Sandbox
    {
        internal static void Initialize(ISandbox sandbox)
        {
            if (sandbox == null)
            {
                return;
            }

            foreach (var file in Directory.GetFiles(Folder.ThirdParty).Where(p => Path.GetExtension(p) == ".dll"))
            {
                try
                {
                    var assembly = sandbox.LoadAssembly(file);

                    foreach (var type in assembly.GetTypes()/*.Where(type => type.IsSubclassOf(typeof(IScript)*/)
                    {
                        Activator.CreateInstance(type);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }
    }
}