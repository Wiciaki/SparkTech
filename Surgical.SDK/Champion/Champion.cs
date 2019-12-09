namespace Surgical.SDK.Champion
{
    using Newtonsoft.Json.Linq;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.Modules;

    public class Champion : IModule
    {
        public Menu Menu { get; } = new Menu("surgical") { Text = "Not implemented" };

        public virtual float GetHealthIndicatorDamage(IHero hero)
        {
            return 0;
        }

        public virtual JObject GetTranslations()
        {
            return null;
        }

        public virtual void Start()
        {

        }

        public virtual void Pause()
        {

        }
    }
}