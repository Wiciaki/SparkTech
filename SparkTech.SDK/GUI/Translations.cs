namespace SparkTech.SDK.GUI
{
    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Logging;

    public class Translations
    {
        private static readonly string DefaultTag;

        private readonly JObject translations;

        static Translations()
        {
            DefaultTag = EnumCache<Language>.Description(default);
        }

        public Translations(JObject o)
        {
            this.translations = o;
        }

        public JToken GetToken(string str)
        {
            return this.translations[Menu.Menu.LanguageTag]?[str] ?? this.translations[DefaultTag]?[str];
        }

        public string GetString(string str)
        {
            return this.GetToken(str)?.Value<string>();
        }

        public Translations GetObject(string id)
        {
            return new Translations((JObject)this.translations[id]);
        }
    }
}