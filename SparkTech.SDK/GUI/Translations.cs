namespace SparkTech.SDK.GUI
{
    using Newtonsoft.Json.Linq;

    public class Translations
    {
        private readonly JObject translations;

        public Translations(JObject o)
        {
            this.translations = o;
        }

        public JToken GetToken(string str)
        {
            return this.translations?[Menu.Menu.LanguageTag]?[str];
        }

        public string GetString(string str)
        {
            return this.GetToken(str)?.Value<string>();
        }

        public Translations GetObject(string id)
        {
            return new Translations((JObject)this.translations?[id]);
        }
    }
}