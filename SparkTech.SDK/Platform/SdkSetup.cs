namespace SparkTech.SDK.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Misc;
    using SparkTech.SDK.Modules;
    using SparkTech.SDK.SpellDatabase;
    using SparkTech.SDK.TickOperations;
    using SparkTech.SDK.UI;
    using SparkTech.SDK.UI.Menu;
    using SparkTech.SDK.UI.Menu.Values;
    using SparkTech.SDK.UI.Notifications;
    using SparkTech.SDK.Util;

    internal static class SdkSetup
    {
        #region Static Fields

        private static readonly string ModulesDirectory;

        private static readonly RootMenu RootMenu;

        private static readonly Menu ModuleSelectionMenu;

        private static Menu ModulesMenu => RootMenu;

        private const string DefaultItemName = "Default";

        internal static readonly string SaveDirectory;

        #endregion

        #region Constructors and Destructors

        static SdkSetup()
        {
            var folder = Paths.GetScriptDataFolder();

            ModulesDirectory = Path.Combine(folder, "Modules");
            Directory.CreateDirectory(ModulesDirectory);

            SaveDirectory = Path.Combine(folder, "MenuData");
            Directory.CreateDirectory(SaveDirectory);

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

            var flagFile = Path.Combine(folder, ".firstrun");

            if (!File.Exists(flagFile))
            {
                File.Create(flagFile).Dispose();

                Logging.Log("Running Entropy.SDK for the first time.");

                var code = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
                var values = EnumCache<Language>.Values;
                var i = values.ConvertAll(EnumCache<Language>.Description).IndexOf(code);

                if (i == -1)
                {
                    Notification.Send(
                        15f,
                        $"Welcome to Entropy!\nYour system language,\n\"{code}\" is not supported by the SDK.\n"
                                                + "English will be used.\nYou can change that inside the menu.");
                }
                else
                {
                    lang.Value = i;

                    Notification.Send(10f, GetNotificationMessage());

                    string GetNotificationMessage()
                    {
                        switch (values[i])
                        {
                            case Language.Polish:
                                return "Witamy na pokładzie!\nWykryto język polski.\nMożesz to zmienić w menu.";

                            default:
                                return "Welcome to Entropy!\nEnglish was detected as the best language for you.\nYou can always change that in the menu.";
                        }
                    }
                }
            }

            RootMenu.Add(lang);
            RootMenu.Add(new MenuImage("Logo").SetImage(Resources.banner_purple_bright));

            Subscribe(LangChanged, lang);
            Subscribe(ClockUpdated, clock["DrawClock"], clock["x"], clock["y"]);
            Subscribe(() => Hacks.AntiAFK = antiAfk.GetValue<bool>(), antiAfk);
            Subscribe(() => SdkSetup.RootMenu.IsMovable = movable.GetValue<bool>(), movable);
            Subscribe(() => GameTick.TicksPerSecond = ticks.GetValue<int>(), ticks);

            button.PropertyChanged += (o, args) => ControlChanged(args.PropertyName == "Key");
            toggle.PropertyChanged += (o, args) => ControlChanged(args.PropertyName == "IsActive");
            ControlChanged(true);

            CreateTest();

            Logging.Log("Entropy.SDK - loaded!");

            void Subscribe(Action callback, params MenuValue[] notifiers)
            {
                Array.ForEach(notifiers, m => m.PropertyChanged += (o, args) => callback());

                callback();
            }

            #region Callbacks

            async void SaveTemplate()
            {
                await RootMenu.SaveTranslationTemplate(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), RootMenu.Id + ".json"));
            }

            void ClockUpdated()
            {
                Notification.DrawClock = clock["DrawClock"].GetValue<bool>();
                Notification.ExtraClockSize = new Size(clock["x"].GetValue<int>(), clock["y"].GetValue<int>());
            }

            void ControlChanged(bool b)
            {
                if (b)
                {
                    Menu.SetBehavior(button.GetValue<WindowMessageWParam>(), toggle.GetValue<bool>());
                }
            }

            void LangChanged()
            {
                Menu.SetLanguage(lang.Enum<Language>());

                var resource = (byte[])Resources.ResourceManager.GetObject(Menu.Language.ToString());

                if (resource == null)
                {
                    Logging.Log($"Translation file for {Menu.Language} language couldn't be found in Entropy.SDK!", LogLevels.Warn);

                    return;
                }

                RootMenu.Translate(JObject.Parse(Encoding.UTF8.GetString(resource)));
            }

            #endregion
        }

        private static void CreateTest()
        {
            var i = 0;
            var n = new Notification { Time = 5f, Content = "This is a test\nnotification", Description = "OH FUCK IT WORKS" };

            new RootMenu("sdkTest")
            {
                new MenuButton("btn", () => GameConsole.Print($"You pressed a button {++i} times!", false))
                    .SetDisplayName("Some button"),
                new Menu("xd")
                {
                    new MenuLabel("xd").SetDisplayName("this text is valuable"),
                    new MenuLabel("xd2").SetDisplayName("hoooooly shit this is so important"),
                    new MenuButton(
                            "xd4",
                            () => GameConsole.Print($"Button pressed! Game.ClockTime equals {Game.ClockTime}", false))
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
                new MenuKeyBind("xd10", WindowMessageWParam.Z) { Toggle = true }.SetDisplayName("Press a button"),
                new MenuColor("xdColor", Color.Magenta).SetDisplayName("Circle color")
            }.SetDisplayName("SDK test menu");

            GameLoading.OnLoad += () => Renderer.OnRender += TestRender;
        }

        private static readonly PlayerSpell Q = new PlayerSpell(SpellSlot.Q);

        private static void TestRender(EntropyEventArgs args)
        {
            var text = "Q ready in: " + Q.GetTimeUntilReady();

            var pos = Renderer.WorldToScreen(LocalPlayer.Instance.Position).ToPoint();

            Theme.Draw(new DrawData(pos, Theme.MeasureSize(text)) { Text = text, ForceTextCentered = true });
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
            using (var fileStream = new FileStream(targetPath, File.Exists(targetPath) ? FileMode.Truncate : FileMode.Create, FileAccess.Write, FileShare.None))
            using (var streamWriter = new StreamWriter(fileStream))
            using (var testWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented })
            {
                await token.WriteToAsync(testWriter);
            }
        }

        private static string GetDisplayableTypeName<T>()
        {
            var name = typeof(T).Name;

            return Regex.IsMatch(name, "^I[A-Z]") ? name.Substring(1) : name;
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

        private sealed class MenuItemPicker<TModuleBase> : IModulePicker<TModuleBase>
            where TModuleBase : class, IModule
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
                    ex.LogException(
                        $"Couldn't create an instance of the menu selected module \"{moduleType.FullName}\". "
                        + "This is the script dev's fault. Please contact him about this error.");

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
                    var target = Path.Combine(ModulesDirectory, this.GetUniqueInstanceName() + ".json");

                    if (File.Exists(target))
                    {
                        try
                        {
                            menu.SetToken(JObject.Parse(File.ReadAllText(target)));
                        }
                        catch (Exception ex)
                        {
                            ex.LogException($"Couldn't parse the JSON config for ModuleMenu \"{menu.DisplayName}\"!");
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

                Logging.Log("Saving the updated config for " + name + "...");

                var target = Path.Combine(ModulesDirectory, name + ".json");
                var token = menu.GetModuleToken();

                if (token == null)
                {
                    if (File.Exists(target))
                    {
                        File.Delete(target);
                    }

                    Logging.Log("Module data was at the default values.");
                }
                else
                {
                    await SaveToFileAsync(target, token);

                    Logging.Log("Saved the module data successfully.");
                }
            }

            private string GetUniqueInstanceName()
            {
                return GetDisplayableTypeName<TModuleBase>() + " - " + this.CurrentModuleName;
            }

            #endregion
        }
    }
}