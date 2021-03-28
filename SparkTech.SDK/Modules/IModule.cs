namespace SparkTech.SDK.Modules
{
    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.GUI.Menu;

    public interface IModule : IEntryPoint, IResumable
    {
        Menu Menu { get; }

        JObject GetTranslations();
    }
}