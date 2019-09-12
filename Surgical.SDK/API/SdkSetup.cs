namespace Surgical.SDK.API
{
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SharpDX.Direct3D9;

    using Surgical.SDK.GUI;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.GUI.Notifications;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Properties;
    using Surgical.SDK.Rendering;
    using Surgical.SDK.Security;

    public static class SdkSetup
    {
        public static readonly bool FirstRun;

        private static readonly Menu Menu;

        private static readonly Translations Strings;

        static SdkSetup()
        {
            var texture = Texture.FromMemory(Render.Device, Resources.Banner, 295, 100, 0, Usage.None, Format.Unknown, Pool.Default, Filter.Default, Filter.Default, 0);

            Strings = new Translations(JObject.Parse(Resources.Strings));

            Menu = new Menu("sdk")
            {
                new MenuList("language") { Options = EnumCache<Language>.Names },
                new Menu("modes"),
                new Menu("menu")
                {
                    new MenuKey("key", Key.LeftShift),
                    new MenuBool("toggle", false),
                    new MenuAction("apply", SetMenuTriggers),
                    new MenuBool("arrows", true),
                    new MenuText("position"),
                    new MenuInt("x", 0, 500, 40),
                    new MenuInt("y", 0, 500, 40)
                },
                new MenuList("clock"),
                new Menu("humanizer")
                {
                    new MenuBool("enable", true)
                },
                new Menu("license"),
                new Menu("about")
                {
                    new MenuText("version"),
                    new MenuText("contact"),
                    new MenuText("credits")
                },
                new MenuTexture("banner", texture)
            };

            Mode.Initialize(Menu.GetMenu("modes"));

            Menu.Build(Menu, GetMenuTranslations());
            
            SetupMenu(out FirstRun);

            Log.Info("SDK building complete!");

            // test

            Menu.IsExpanded = true;
            //Menu.GetMenu("about").IsExpanded = true;
            //Menu.GetMenu("modes").GetMenu("harass").IsExpanded = true;
            //((IExpandable)Menu.GetMenu("modes").GetMenu("harass")["minions"]).IsExpanded = true;
            //((IExpandable)Menu["clock"]).IsExpanded = true;

            Menu.Build(new Menu("Evade"));
            Menu.Build(new Menu("Nocturne"));

            //Menu["clock"].SetValue(1);
            //Menu.GetMenu("modes").GetMenu("harass")["champAuto"].SetValue(false);

            Menu.SetOpen(true);
        }

        #region Menu Setup

        private static void SetupMenu(out bool firstRun)
        {
            HandlePosition();
            HandleClock();
            HandleArrows();

            SetMenuTriggers();

            Menu["language"].BeforeValueChange += args => Menu.SetLanguage(args.NewValue<int>());

            var flagFile = Folder.Menu.GetFile(".nofirstrun");
            firstRun = !File.Exists(flagFile);

            if (!firstRun)
            {
                Menu.SetLanguage(Menu["language"].GetValue<int>());
                return;
            }

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

        internal static string GetTranslatedString(string str)
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

            item.BeforeValueChange += args => Menu.SetArrows(args.NewValue<bool>());

            Menu.SetArrows(item.GetValue<bool>());
        }

        private static void SetMenuTriggers()
        {
            var menu = Menu.GetMenu("menu");

            var key = menu["key"].GetValue<Key>();
            var toggle = menu["toggle"].GetValue<bool>();

            Menu.SetTriggers(key, toggle);
        }

        private static JObject GetMenuTranslations()
        {
            var str = Resources.MainMenu;
            
            str = str.Replace("{version}", "1.0.0.0");

            var mainMenu = JObject.Parse(str);
            var mode = JObject.Parse(Resources.Mode);

            foreach (var o in mainMenu["modes"].Skip(EnumCache<Language>.Values.Count).Take(Mode.ModeCount).Cast<JProperty>().Select(prop => prop.Value).Cast<JObject>())
            {
                foreach (var pair in mode)
                {
                    o.Add(pair.Key, pair.Value);
                }
            }

            return mainMenu;
        }

        #endregion

        /*
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