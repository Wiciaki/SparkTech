namespace SharpLinker
{
    using System;
    using System.IO;
    using System.Reflection;

    using Surgical.SDK.API.Fragments;

    internal class Sandbox : ISandbox
    {
        public Assembly LoadAssembly(string path)
        {
            return Assembly.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(path)));
        }
    }
}