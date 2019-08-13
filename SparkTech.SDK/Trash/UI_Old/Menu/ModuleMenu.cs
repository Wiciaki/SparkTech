namespace SparkTech.SDK.UI_Old.Menu
{
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    public class ModuleMenu : Menu
    {
        public ModuleMenu(string id) : base(id)
        {

        }

        protected internal override void UpdateTranslations(JToken token)
        {

        }

        public void Translate(JObject jObject)
        {
            base.UpdateTranslations(jObject);
        }

        protected internal override JObject GetRequiredTranslations()
        {
            return null;
        }

        public async Task SaveTranslationTemplate(string targetPath)
        {
            await EntropySetup.SaveToFileAsync(targetPath, base.GetRequiredTranslations());
        }

        protected internal override JToken GetToken() => null;

        internal JToken GetModuleToken() => base.GetToken();

        protected internal override bool ShouldSave() => false;

        internal bool ModuleValueChanged() => base.ShouldSave();
    }
}