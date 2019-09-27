namespace Surgical.SDK.Modules
{
    using Newtonsoft.Json.Linq;

    using Surgical.SDK.GUI.Menu;

    public interface IModule
    {
        Menu Menu { get; }

        JObject GetTranslations();

        void Start();

        void Stop();
    }
}