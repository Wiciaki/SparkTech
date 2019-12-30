namespace Surgical.SDK.Evade
{
    using Newtonsoft.Json.Linq;

    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.Properties;

    public class Evade : IEvade
    {
        public Menu? Menu { get; } = new Menu("surgical");

        public JObject? GetTranslations()
        {
            return JObject.Parse(Resources.Evade);
        }

        public void Start()
        {
            
        }

        public void Pause()
        {

        }
    }
}