namespace Surgical.SDK.GUI
{
    using System.Collections;
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    public sealed class Translations : IEnumerable
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

        public void Add(JObject value)
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

            return o == null && !this.customs.TryGetValue(id, out o) ? null : new Translations { o };
        }

        public IEnumerator GetEnumerator()
        {
            return new[] { this.translations }.GetEnumerator();
        }
    }
}