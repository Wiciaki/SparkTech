﻿namespace Surgical.SDK
{
    using System.Collections.Generic;
    using System.Linq;

    using Surgical.SDK.API;
    using Surgical.SDK.GUI.Menu;

    public class Mode
    {
        public static Mode CurrentMode => Modes.Find(mode => mode.IsActive);

        public virtual string DisplayName => this.menu.Text;

        protected virtual bool IsActive => this.GetConfig("key");

        public bool ChampsPokeSpells => this.GetConfig("champSpells", 1);

        public bool ChampsAllSpells => this.GetConfig("champSpells", 2);

        public bool ChampsAutoAttack => this.GetConfig("champAuto");

        public bool MinionsFarm => this.GetConfig("minions", 1);

        public bool MinionsPush => this.GetConfig("minions", 2);

        public bool AttackWards => this.GetSubConfig("objects", "wards");

        public bool AttackClones => this.GetSubConfig("objects","clones");

        public bool AttackPets => this.GetSubConfig("objects", "pets");

        public bool AttackBarrel => this.GetSubConfig("objects","barrel");

        internal static int ModeCount => Modes.Count - 1;

        private static readonly List<Mode> Modes = new List<Mode>();

        private readonly Menu menu;

        protected Mode()
        {

        }

        private Mode(string menuName, ModeSettings settings) : this()
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

            foreach (var item in this.menu.GetDescensants().OfType<MenuValue>())
            {
                item.IsChampSpecific = true;
            }

            this.menu.Get<MenuValue>("key").IsChampSpecific = false;
        }

        protected virtual bool GetConfig(string id)
        {
            return this.menu[id].GetValue<bool>();
        }

        protected virtual bool GetSubConfig(string subMenuId, string id)
        {
            return this.menu.GetMenu(subMenuId)[id].GetValue<bool>();
        }

        protected virtual bool GetConfig(string id, int minIndex)
        {
            return this.menu[id].GetValue<int>() >= minIndex;
        }

        internal static void Initialize(Menu menu)
        {
            var dictionary = new Dictionary<string, ModeSettings>
                       {
                           ["combo"] = new ModeSettings(Key.Space) { ChampionAuto = true, ChampionSpells = 2 },
                           ["laneclear"] = new ModeSettings(Key.V) { ChampionAuto = true, Minions = 2, Pets = true, Wards = true },
                           ["harass"] = new ModeSettings(Key.C) { ChampionAuto = true, ChampionSpells = 1, Minions = 1, Wards = true },
                           ["lasthit"] = new ModeSettings(Key.X) { Minions = 1, Wards = true },
                           ["custom"] = new ModeSettings(Key.Z),
                           ["flee"] = new ModeSettings(Key.A) { Flee = true, Explosives = true }
                       };

            Modes.AddRange(dictionary.Select(pair => new Mode(pair.Key, pair.Value)));
            Modes.ForEach(mode => menu.Add(mode.menu));

            Modes.Add(new NoneMode());
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

        private class NoneMode : Mode
        {
            public override string DisplayName => SdkSetup.GetTranslatedString("none");

            protected override bool IsActive { get; } = true;

            protected override bool GetConfig(string id) => false;

            protected override bool GetSubConfig(string subMenuId, string id) => false;

            protected override bool GetConfig(string id, int minIndex) => false;
        }
    }
}