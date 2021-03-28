namespace SparkTech.SDK.Champion
{
    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.Modules;
    using SparkTech.SDK.Properties;

    public static class ChampionService
    {
        public static readonly Picker<IChampion> Picker = new Picker<IChampion>(GetChampionModule());

        private static IChampion GetChampionModule()
        {
            var charName = string.Empty;

            if (Platform.HasCoreAPI)
            {
                //charName = ObjectManager.Player.CharName;
            }

            switch (charName)
            {
                case "Orianna":
                    return new Orianna();
                case "Vayne":
                    return new Vayne();
                case "Viktor":
                    return new Viktor();
                case "Xerath":
                    return new Xerath();
                case "Jinx":
                    return new Jinx();
                default:
                    return new NullChampion();
            }
        }

        private class NullChampion : IChampion
        {
            void IResumable.Start()
            { }

            void IResumable.Pause()
            { }

            Menu IModule.Menu { get; } = new Menu("notimplemented");

            JObject IModule.GetTranslations() => JObject.Parse(Resources.NullChampion);

            float IChampion.GetHealthIndicatorDamage(IHero hero) => 0f;
        }
    }
}