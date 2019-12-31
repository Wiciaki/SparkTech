namespace Surgical.SDK.Modules
{
    using Newtonsoft.Json.Linq;

    using Surgical.SDK.GUI.Menu;

    public interface IModule : IEntryPoint, IResumable
    {
        Menu Menu { get; }

        JObject? GetTranslations();
    }
}