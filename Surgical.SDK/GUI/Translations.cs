namespace Surgical.SDK.GUI
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    public sealed class Translations
    {
        private JObject translations = new JObject();

        private readonly Dictionary<string, JObject> customs = new Dictionary<string, JObject>();

        public JToken GetToken(string str)
        {
            return this.translations[Menu.Menu.LanguageTag]?[str];
        }

        public string GetString(string str)
        {
            return this.GetToken(str)?.Value<string>();
        }

        public void Set(JObject value)
        {
            this.translations = value;
        }

        public void Add(string id, JObject value)
        {
            this.customs.Add(id, value);
        }

        public Translations GetObject(string id)
        {
            var o = (JObject)this.translations[id];

            if (o == null && !this.customs.TryGetValue(id, out o))
            {
                return null;
            }

            var t = new Translations();
            t.Set(o);

            return t;
        }
    }
}