namespace Surgical.SDK.Modules
{
    using Newtonsoft.Json.Linq;

    using Surgical.SDK.GUI.Menu;

    public interface IModule : IEntryPoint
    {
        Menu Menu { get; }

        JObject GetTranslations();

        void Start();

        void Pause();
    }
}