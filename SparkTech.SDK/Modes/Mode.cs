namespace SparkTech.SDK.Modes
{
    using System.Collections.Generic;
    using System.Linq;

    using SparkTech.SDK.GUI.Menu;

    public class Mode
    {
        public static Mode GetCurrentMode()
        {
            return Modes.Find(mode => mode.IsActive);
        }

        internal const int ModeCount = 6;

        private static readonly List<Mode> Modes = new List<Mode>();

        private readonly Menu menu;

        private Mode(string menuName, ModeSettings settings)
        {
            this.menu = new Menu(menuName)
                        {
                            new MenuKeyBool("key", settings.Key),
                            new MenuList("champSpells", settings.ChampionSpells),
                            new MenuBool("champAuto", settings.ChampionAuto),
                            new MenuList("minions", settings.Minions),
                            new Menu("objects")
                            {
                                new MenuBool("wards", settings.Wards),
                                new MenuBool("clones", settings.Pets),
                                new MenuBool("pets", settings.Pets),
                                new MenuBool("barrel", settings.Pets)
                            },
                            new MenuBool("flee", settings.Flee),
                            new MenuBool("explosives", settings.Explosives)
                        };

            foreach (var item in this.menu.GetDescensants().OfType<MenuValue>().Where(item => item.Id != "key"))
            {
                item.IsChampSpecific = true;
            }
        }

        private bool GetConfig(string str)
        {
            return this.menu[str].GetValue<bool>();
        }

        private bool GetObjectsConfig(string str)
        {
            return this.menu.GetMenu("pets")[str].GetValue<bool>();
        }

        private bool GetConfig(string str, int minIndex)
        {
            return this.menu[str].GetValue<int>() >= minIndex;
        }

        public bool IsActive => this.GetConfig("key");

        public string Name => this.menu.Text;

        public bool ChampsPokeSpells => this.GetConfig("champSpells", 1);

        public bool ChampsAllSpells => this.GetConfig("champSpells", 2);

        public bool ChampsAutoAttack => this.GetConfig("champAuto");

        public bool MinionsFarm => this.GetConfig("minions", 1);

        public bool MinionsPush => this.GetConfig("minions", 2);

        public bool AttackWards => this.GetObjectsConfig("wards");

        public bool AttackClones => this.GetObjectsConfig("clones");

        public bool AttackPets => this.GetObjectsConfig("pets");

        public bool AttackBarrel => this.GetObjectsConfig("barrel");

        internal static void Initialize(Menu menu)
        {
            var dict = new Dictionary<string, ModeSettings>
                       {
                           ["combo"] = new ModeSettings(Key.Space) { ChampionAuto = true, ChampionSpells = 2 },
                           ["laneclear"] = new ModeSettings(Key.V) { ChampionAuto = true, Minions = 2, Pets = true, Wards = true },
                           ["harass"] = new ModeSettings(Key.C) { ChampionAuto = true, ChampionSpells = 1, Minions = 1, Wards = true },
                           ["lasthit"] = new ModeSettings(Key.X) { Minions = 1, Wards = true },
                           ["custom"] = new ModeSettings(Key.Z),
                           ["flee"] = new ModeSettings(Key.A) { Flee = true, Explosives = true }
                       };

            foreach (var mode in dict.Select(pair => new Mode(pair.Key, pair.Value)))
            {
                menu.Add(mode.menu);

                Modes.Add(mode);
            }
        }

        private class ModeSettings
        {
            public readonly Key Key;

            public int ChampionSpells, Minions;

            public bool ChampionAuto, Wards, Pets, Explosives, Flee;

            public ModeSettings(Key key)
            {
                this.Key = key;
            }
        }
    }
}