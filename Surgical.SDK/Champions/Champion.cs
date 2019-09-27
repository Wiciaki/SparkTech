namespace Surgical.SDK.Champions
{
    using Newtonsoft.Json.Linq;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.Modules;

    public abstract class Champion : IModule
    {
        public Menu Menu { get; }

        protected Champion()
        {
            var champName = ObjectManager.Player.CharName;

            this.Menu = new Menu(champName.ToLower())
                        {

                        };

            var a = this.GetTranslations();
        }

        public virtual float GetHealthIndicatorDamage(IHero hero)
        {
            return 0;
        }

        internal static void Initialize()
        {
            var champName = ObjectManager.Player.CharName;

            var champion = GetChampion(champName);


        }

        private static Champion GetChampion(string champName)
        {
            return champName switch
            {
                "Orianna" => new Orianna(),
                "Vayne" => new Vayne(),
                "Viktor" => new Viktor(),
                "Xerath" => new Xerath(),
                _ => null as Champion
            };
        }

        //public abstract JObject GetTranslations();
        public virtual JObject GetTranslations() => null;

        public virtual void Start()
        {

        }

        public virtual void Stop()
        {

        }
    }
}