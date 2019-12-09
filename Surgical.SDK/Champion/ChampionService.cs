namespace Surgical.SDK.Champion
{
    using Surgical.SDK.Entities;
    using Surgical.SDK.Modules;

    public static class ChampionService
    {
        public static readonly Picker<Champion> Picker = new Picker<Champion>(GetModule());

        private static Champion GetModule()
        {
            return /*ObjectManager.Player.CharName*/ "" switch
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