namespace Surgical.SDK.API.Fragments
{
    using System.Reflection;

    public interface ISandbox
    {
        Assembly LoadAssembly(string path);
    }
}