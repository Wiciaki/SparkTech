﻿namespace SparkTech.SDK.API
{
    using System.Globalization;
    using System.IO;

    using Newtonsoft.Json.Linq;

    using SharpDX.Direct3D9;

    using SparkTech.SDK.GUI;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.GUI.Notifications;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Properties;
    using SparkTech.SDK.Rendering;
    using SparkTech.SDK.Security;

    public static class SdkSetup
    {
        public static readonly bool FirstRun;

        private static readonly Menu Menu;

        private static readonly Translations Strings;

        static SdkSetup()
        {
            var texture = Texture.FromMemory(Render.Device, Resources.Banner, 287, 97, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);

            Strings = new Translations(JObject.Parse(Resources.Strings));

            Menu = new Menu("sdk")
            {
                new MenuList("language") { Options = EnumCache<Language>.Names },
                new Menu("menu")
                {
                    new MenuKey("key", Key.LeftShift),
                    new MenuBool("toggle", false),
                    new MenuAction("apply") { Action = SetMenuTriggers },
                    new MenuBool("arrows", true),
                    new MenuSeparator("separator1"),
                    new MenuInt("x", 0, 500, 40),
                    new MenuInt("y", 0, 500, 40)
                },
                new MenuList("clock"),
                new Menu("humanizer")
                {
                    new MenuBool("enable", true)
                },
                new MenuTexture("banner") { Texture = texture }
            };

            Menu.Build(Menu, JObject.Parse(Resources.MainMenu));
            
            SetupMenu(out FirstRun);

            Log.Info("SDK building complete!");

            Menu.IsExpanded = true;
            Menu.GetMenu("menu").IsExpanded = true;
            //((IExpandable)Menu["clock"]).IsExpanded = true;

            //Menu.GetMenu("position")["x"].SetValue(270);

            Menu.Build(new Menu("Evade"));
            Menu.Build(new Menu("Nocturne"));

            //Menu["language"].SetValue(1);

            Menu.SetOpen(true);
        }

        #region Menu Setup

        private static void SetupMenu(out bool firstRun)
        {
            #region First Run and Language stuff

            Menu["language"].BeforeValueChange += LanguageChanged;

            var flagFile = Folder.Menu.GetFile(".nofirstrun");
            firstRun = !File.Exists(flagFile);

            if (firstRun)
            {
                File.Create(flagFile).Dispose();

                Log.Info("Running Surgical.SDK for the first time.");

                var culture = CultureInfo.InstalledUICulture;
                var values = EnumCache<Language>.Values;
                var i = values.ConvertAll(EnumCache<Language>.Description).IndexOf(culture.TwoLetterISOLanguageName);

                string welcomeMsg;

                if (i == -1)
                {
                    welcomeMsg = GetTranslatedString("languageUnknown");
                    welcomeMsg = welcomeMsg.Replace("{language}", culture.EnglishName);
                }
                else
                {
                    Menu["language"].SetValue(i);

                    welcomeMsg = GetTranslatedString("firstTimeWelcome");
                }

                welcomeMsg = welcomeMsg.Replace("{platform}", Platform.PlatformName);

                Notification.Send(welcomeMsg, 10f);
            }
            else
            {
                var language = (Language)Menu["language"].GetValue<int>();

                Menu.SetLanguage(language);

                Notification.Send("Gandhi is one of\nthe few legit reversers\nleft in the community", "Notification system test");
                Notification.Send("Made with <3 by S H A R K dev team");
            }

            #endregion

            HandlePosition();
            HandleClock();
            HandleArrows();

            SetMenuTriggers();
        }

        public static string GetTranslatedString(string str)
        {
            return Strings.GetString(str);
        }

        private static void HandleClock()
        {
            var clockItem = Menu["clock"];

            clockItem.BeforeValueChange += args => Clock.SetMode(args.NewValue<int>());

            Clock.SetMode(clockItem.GetValue<int>());
        }

        private static void HandlePosition()
        {
            var menu = Menu.GetMenu("menu");

            var x = menu["x"];
            var y = menu["y"];

            x.BeforeValueChange += args => Menu.SetPosition(args.NewValue<int>(), y.GetValue<int>());
            y.BeforeValueChange += args => Menu.SetPosition(x.GetValue<int>(), args.NewValue<int>());

            Menu.SetPosition(x.GetValue<int>(), y.GetValue<int>());
        }

        private static void HandleArrows()
        {
            var item = Menu.GetMenu("menu")["arrows"];

            item.BeforeValueChange += args => MenuText.SetArrows(args.NewValue<bool>());

            MenuText.SetArrows(item.GetValue<bool>());
        }

        private static void SetMenuTriggers()
        {
            var menu = Menu.GetMenu("menu");

            var key = menu["key"].GetValue<Key>();
            var toggle = menu["toggle"].GetValue<bool>();

            Menu.SetTriggers(key, toggle);
        }

        private static void LanguageChanged(BeforeValueChangeEventArgs args)
        {
            var language = (Language)args.NewValue<int>();

            Menu.SetLanguage(language);
        }

        #endregion

        /*
        #region Static Fields

        private static readonly Folder ModulesDirectory;

        private static readonly RootMenu RootMenu;

        private static readonly Menu ModuleSelectionMenu;

        private static Menu ModulesMenu => RootMenu;

        private const string DefaultItemName = "Default";

        internal static readonly Folder SaveDirectory;

        #endregion

        #region Constructors and Destructors

        static SdkSetup()
        {
            ModulesDirectory = Folder.MenuFolder.GetFolder("Modules");

            SaveDirectory = Path.Combine(folder, "MenuData");

            RootMenu = new RootMenu("Entropy.SDK");
            ModuleSelectionMenu = RootMenu.Add(new Menu("Modules"));

            var hacks = RootMenu.Add(new Menu("Hacks"));
            var antiAfk = hacks.Add(new MenuCheckBox("AntiAfk", false));
            var clock = RootMenu.Add(new Menu("Clock")
                                     {
                                         new MenuCheckBox("DrawClock", true),
                                         new MenuSlider("x", 0, 75, -25),
                                         new MenuSlider("y", 10, -25, 75)
                                     });
            var ticks = RootMenu.Add(new MenuSlider("Ticks", 25, 10, 50));
            var movable = RootMenu.Add(new MenuCheckBox("MenuMovable", false));
            var lang = new MenuList("Language", EnumCache<Language>.Names);
            var button = RootMenu.Add(new MenuKeyBind("MenuButton", Keys.Shift));
            var toggle = RootMenu.Add(new MenuCheckBox("MenuToggle", false));
            RootMenu.Add(new MenuButton("SaveTemplate", SaveTemplate));

            RootMenu.Add(lang);
            RootMenu.Add(new MenuImage("Logo").SetImage(Resources.banner_purple_bright));

            Subscribe(LangChanged, lang);
            Subscribe(ClockUpdated, clock["DrawClock"], clock["x"], clock["y"]);
            Subscribe(() => Hacks.AntiAFK = antiAfk.GetValue<bool>(), antiAfk);
            Subscribe(() => RootMenu.IsMovable = movable.GetValue<bool>(), movable);
            Subscribe(() => GameTick.TicksPerSecond = ticks.GetValue<int>(), ticks);

            button.PropertyChanged += (o, args) => ControlChanged(args.PropertyName == "Key");
            toggle.PropertyChanged += (o, args) => ControlChanged(args.PropertyName == "IsActive");
            ControlChanged(true);

            CreateTest();

            Log.Info("SparkTech.SDK - loaded!");

            #endregion
        }

        private static void CreateTest()
        {
            var i = 0;
            var n = new Notification { Time = 5f, Content = "This is a test\nnotification", Description = "OH FUCK IT WORKS" };

            new RootMenu("sdkTest")
            {
                new MenuButton("btn", () => GameInterface.Show($"You pressed a button {++i} times!"))
                    .SetDisplayName("Some button"),
                new Menu("xd")
                {
                    new MenuLabel("xd").SetDisplayName("this text is valuable"),
                    new MenuLabel("xd2").SetDisplayName("hoooooly shit this is so important"),
                    new MenuButton(
                            "xd4",
                            () => GameInterface.Print($"Button pressed! Game.ClockTime equals {SDK.Game.ClockTime}", false))
                        .SetDisplayName("Press me! :)"),
                    new MenuButton("xdNotif", () => n.Send()).SetDisplayName("Spawn notification"),
                    new MenuList(
                            "xd5",
                            new List<string> { "option", "a longer option", "other option" })
                        .SetDisplayName("I may look like a menu, but I'm a list!"),
                    new Menu("xd6") { new MenuLabel("xd").SetDisplayName("XD") }.SetDisplayName("Test")
                }.SetDisplayName("testing menu"),
                new MenuCheckBox("xd2", false).SetDisplayName("Button test")
                    .SetTooltipText("This is a useful information."),
                new MenuCheckBox("xd3", true).SetDisplayName("Button test"),
                new MenuSlider("xd4", 20).SetDisplayName("Select a value"),
                new MenuKeyBind("xd10", WindowsMessagesWParam.Z) { Toggle = true }.SetDisplayName("Press a button"),
                new MenuColor("xdColor", Color.Magenta).SetDisplayName("Circle color")
            }.SetDisplayName("SDK test menu");

            GameLoading.OnLoad += () => Render.OnDraw += TestRender;
        }

        private static readonly PlayerSpell Q = new PlayerSpell(SpellSlot.Q);

        private static void TestRender()
        {
            var text = "Q ready in: " + Q.GetTimeUntilReady();

            var pos = ObjectManager.Player.Position();

            Text.Draw(text, SharpDX.Color.Red, pos.WorldToScreen());
        }

        #endregion

        #region Methods

        internal static IModulePicker<T> CreatePicker<T, TK>() where T : class, IModule where TK : T, new()
        {
            var picker = new MenuItemPicker<T>(ModuleSelectionMenu.Add(new ModuleList(GetDisplayableTypeName<T>())), new TK());

            picker.Add<TK>(DefaultItemName);

            return picker;
        }

        internal static async Task SaveToFileAsync(string targetPath, JToken token)
        {
            await using var fileStream = new FileStream(targetPath, File.Exists(targetPath) ? FileMode.Truncate : FileMode.Create, FileAccess.Write, FileShare.None);
            await using var streamWriter = new StreamWriter(fileStream);
            using var testWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented };

            await token.WriteToAsync(testWriter);
        }

        private static string GetDisplayableTypeName<T>()
        {
            var name = typeof(T).Name;

            return Regex.IsMatch(name, "^I[A-Z]") ? name[1..] : name;
        }

        #endregion

        private sealed class ModuleList : MenuList
        {
            public ModuleList(string id) : base(id, new List<string> { DefaultItemName })
            {

            }

            protected internal override bool ShouldSave() => false;

            protected override JToken Token
            {
                get => 0;
                set { }
            }
        }

        private sealed class ModulePicker<TModuleBase> : IModulePicker<TModuleBase> where TModuleBase : class, IModule
        {
            #region Fields

            private readonly string lastSelected, targetPath;

            private readonly ModuleList item;

            private readonly Dictionary<string, Type> modules;

            private bool userOverride;

            #endregion

            #region Constructors and Destructors

            public MenuItemPicker(ModuleList item, TModuleBase @default)
            {
                this.modules = new Dictionary<string, Type> { { DefaultItemName, typeof(TModuleBase) } };

                this.targetPath = Path.Combine(ModulesDirectory, GetDisplayableTypeName<TModuleBase>());

                if (File.Exists(this.targetPath))
                {
                    this.lastSelected = File.ReadAllText(this.targetPath);
                }

                this.Current = @default;
                this.CurrentModuleName = DefaultItemName;
                ModulesMenu.Add(@default.Menu);

                item.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName != "Selection")
                    {
                        return;
                    }

                    this.userOverride = true;
                    this.Set(item.GetValue<string>());
                };

                this.item = item;

                Menu.VisibilityStateChanged += delegate
                {
                    if (!Menu.IsOpen && this.Current.Menu != null)
                    {
                        this.Save(this.Current.Menu);
                    }
                };
            }

            #endregion

            #region Public Events

            public event Action ModuleSelected;

            #endregion

            #region Public Properties

            public TModuleBase Current { get; private set; }

            public string CurrentModuleName { get; private set; }

            #endregion

            #region Explicit Interface Methods

            public void Add<TModule>(string moduleName) where TModule : TModuleBase, new()
            {
                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    throw new ArgumentException("The supplied module name was null or white space");
                }

                if (this.modules.ContainsKey(moduleName))
                {
                    throw new ArgumentException(
                        $"The module name \"{moduleName}\" was already present in the \"{typeof(TModuleBase).Name}\" picker.");
                }

                this.modules.Add(moduleName, typeof(TModule));
                this.item.List.Add(moduleName);

                if (!this.userOverride && moduleName == this.lastSelected)
                {
                    this.Set(moduleName);
                }
            }

            #endregion

            #region Methods

            private void Set(string moduleName)
            {
                var moduleType = this.modules[moduleName];

                object instance;

                try
                {
                    instance = Activator.CreateInstance(moduleType);
                }
                catch (Exception ex)
                {
                    Log.Error(
                        $"Couldn't create an instance of the menu selected module \"{moduleType.FullName}\". "
                        + "This is the script dev's fault. Please contact him about this error.");

                    ex.Log();

                    return;
                }

                var menu = this.Current.Menu;

                if (menu != null)
                {
                    this.Save(menu);

                    ModulesMenu.Remove(menu);
                }

                this.Current.Release();
                this.Current = (TModuleBase)instance;
                this.CurrentModuleName = moduleName;

                this.item.SetValue(moduleName);

                menu = this.Current.Menu;

                if (menu != null)
                {
                    var target = ModulesDirectory.GetFile(this.GetUniqueInstanceName() + ".json");

                    if (File.Exists(target))
                    {
                        try
                        {
                            menu.SetToken(JObject.Parse(File.ReadAllText(target)));
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Couldn't parse the JSON config for ModuleMenu \"{menu.DisplayName}\"!");
                            ex.Log();
                        }
                    }

                    ModulesMenu.Add(menu);
                }

                if (moduleName == DefaultItemName)
                {
                    var info = new FileInfo(this.targetPath);

                    if (info.Exists)
                    {
                        info.Delete();
                    }
                }
                else
                {
                    File.WriteAllText(this.targetPath, moduleName);
                }

                this.ModuleSelected.SafeInvoke(nameof(this.ModuleSelected));
            }

            private async void Save(ModuleMenu menu)
            {
                if (!menu.ModuleValueChanged())
                {
                    return;
                }

                var name = this.GetUniqueInstanceName();

                Log.Info("Saving the updated config for " + name + "...");

                var target = ModulesDirectory.GetFile(name + ".json");
                var token = menu.GetModuleToken();

                if (token == null)
                {
                    if (File.Exists(target))
                    {
                        File.Delete(target);
                    }

                    Log.Info("Module data was at the default values.");
                }
                else
                {
                    await SaveToFileAsync(target, token);

                    Log.Info("Saved the module data successfully.");
                }
            }

            private string GetUniqueInstanceName()
            {
                return GetDisplayableTypeName<TModuleBase>() + " - " + this.CurrentModuleName;
            }

            #endregion
        }*/
    }
}