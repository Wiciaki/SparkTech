namespace SparkTech.SDK.Evade
{
    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.Properties;

    public class Evade : IEvade
    {
        public Menu Menu { get; } = new Menu("surgical") { new Menu("a") };

        public JObject GetTranslations() => JObject.Parse(Resources.Evade);

        public void Start()
        {
            if (!Platform.HasCoreAPI)
            {
                return;
            }
        }

        public void Pause()
        {

        }
    }
}