namespace Surgical.SDK.Champion
{
    using Surgical.SDK.Entities;
    using Surgical.SDK.Modules;

    public static class ChampionService
    {
        public static readonly Picker<Champion> Picker = new Picker<Champion>(GetChampionModule());

        private static Champion GetChampionModule()
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
                _ => new Champion()
            };
        }
    }
}