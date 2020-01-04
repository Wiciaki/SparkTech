namespace Surgical.SDK.Champion
{
    using Newtonsoft.Json.Linq;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.Modules;
    using Surgical.SDK.Properties;

    public static class ChampionService
    {
        public static readonly Picker<IChampion> Picker = new Picker<IChampion>(GetChampionModule());

        private static IChampion GetChampionModule()
        {
            var charName = string.Empty;

            if (Platform.HasCoreAPI)
            {
                charName = ObjectManager.Player.CharName;
            }

            return charName switch
            {
                "Orianna" => new Orianna(),
                "Vayne" => new Vayne(),
                "Viktor" => new Viktor(),
                "Xerath" => new Xerath(),
                _ => new NullChampion()
            };
        }

        private class NullChampion : IChampion
        {
            void IResumable.Start()
            { }

            void IResumable.Pause()
            { }

            Menu IModule.Menu { get; } = new Menu("notimplemented");

            JObject? IModule.GetTranslations() => JObject.Parse(Resources.NullChampion);

            float IChampion.GetHealthIndicatorDamage(IHero hero) => 0f;
        }
    }
}