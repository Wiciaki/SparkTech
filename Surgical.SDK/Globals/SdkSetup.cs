namespace Surgical.SDK
{
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SharpDX.Direct3D9;

    using Surgical.SDK.Champion;
    using Surgical.SDK.Evade;
    using Surgical.SDK.GUI;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.GUI.Notifications;
    using Surgical.SDK.HealthPrediction;
    using Surgical.SDK.Licensing;
    using Surgical.SDK.Logging;
    using Surgical.SDK.MovementPrediction;
    using Surgical.SDK.Orbwalker;
    using Surgical.SDK.Properties;
    using Surgical.SDK.Rendering;
    using Surgical.SDK.TargetSelector;

    public static class SdkSetup
    {
        public static readonly bool FirstRun;

        private static readonly Menu Menu;

        private static readonly Translations Strings;

        static SdkSetup()
        {
            Strings = new Translations { JObject.Parse(Resources.Strings) };

            Menu = new Menu("sdk")
            {
                new MenuList("language") { Options = EnumCache<Language>.Names },
                new Menu("modes"),
                new Menu("menu")
                {
                    new MenuKey("key", Key.ShiftKey),
                    new MenuBool("toggle", false),
                    new MenuAction("apply", SetMenuTriggers),
                    new MenuBool("arrows", true),
                    new MenuText("position"),
                    new MenuInt("x", 0, 500, 40),
                    new MenuInt("y", 0, 500, 40)
                },
                new MenuList("theme"),
                new Menu("notifications")
                {
                    new MenuFloat("decayTime", 0f, 10f, 4f),
                    new MenuBool("borders", true)
                },
                new MenuList("clock"),
                new Menu("humanizer")
                {
                    new MenuBool("enable", true)
                },
                new Menu("license")
                {
                    new MenuText("lifetime") { IsVisible = false },
                    new MenuText("licensed") { IsVisible = false },
                    new MenuText("unlicensed") { IsVisible = false }
                },
                new Menu("about")
                {
                    new MenuText("version"),
                    new MenuText("contact"),
                    new MenuText("credits")
                }
            };

            if (Platform.HasRenderAPI)
            {
                var texture = Texture.FromMemory(Render.Device, Resources.Banner, 295, 100, 0, Usage.None, Format.Unknown, Pool.Default, Filter.Default, Filter.Default, 0);

                Menu.Add(new MenuTexture("banner", texture));
            }

            Mode.Initialize(Menu.GetMenu("modes")!);
            Menu.Build(Menu, GetMenuTranslations());
            
            SetupMenu(out FirstRun);

            typeof(HealthPredictionService).Trigger();
            typeof(MovementPredictionService).Trigger();
            typeof(TargetSelectorService).Trigger();
            typeof(OrbwalkerService).Trigger();
            typeof(EvadeService).Trigger();
            typeof(ChampionService).Trigger();

            // SurgicalAuth = new Netlicensing(Machine.UserId, "d1213e7b-0817-4544-aa37-01817170c494");
            // var AuthTask = SurgicalAuth.GetAuth("Surgical.SDK").ContinueWith(HandleAuth, TaskScheduler.Current);

            if (!Platform.HasUserInputAPI)
            {
                Menu.IsExpanded = true;
                Menu.GetMenu("menu")!.IsExpanded = true;
            }

            Log.Info("Surgical.SDK initialized!");
        }

        internal static void SetupAuth(AuthResult result)
        {
            var menu = Menu.GetMenu("license")!;

            if (!result.IsLicensed)
            {
                menu["unlicensed"]!.IsVisible = true;
            }
            else if (result.IsLifetime())
            {
                menu["lifetime"]!.IsVisible = true;
            }
            else
            {
                var expiry = result.Expiry.ToString(CultureInfo.InvariantCulture);

                var item = menu.Get<MenuText>("licensed")!;
                item.IsVisible = true;

                item.BindVariable("expiry", expiry);
            }
        }

        private static void SetupMenu(out bool firstRun)
        {
            HandlePosition();
            HandleClock();
            HandleNotifications();
            HandleTheme();
            HandleArrows();

            SetMenuTriggers();

            var langItem = Menu["language"]!;

            langItem.BeforeValueChange += args => Menu.SetLanguage(args.NewValue<int>());

            var flagFile = Folder.Menu.GetFile(".nofirstrun");
            firstRun = !File.Exists(flagFile);

            if (!firstRun)
            {
                Menu.SetLanguage(langItem.GetValue<int>());
                return;
            }

            File.Create(flagFile).Dispose();

            Log.Info("Running Surgical.SDK for the first time.");

            var culture = CultureInfo.InstalledUICulture;
            var languages = EnumCache<Language>.Values;
            var i = languages.ConvertAll(EnumCache<Language>.Description).IndexOf(culture.TwoLetterISOLanguageName);

            string welcomeMsg;

            if (i >= 0)
            {
                langItem.SetValue(i);

                welcomeMsg = GetString("firstTimeWelcome")!;
            }
            else
            {
                welcomeMsg = GetString("languageUnknown")!;
                welcomeMsg = welcomeMsg.Replace("{language}", culture.EnglishName);
            }

            welcomeMsg = welcomeMsg.Replace("{platform}", Platform.Name);

            Notification.Send(welcomeMsg, 10f);
        }

        internal static string? GetString(string str)
        {
            return Strings.GetString(str);
        }

        private static void HandleClock()
        {
            var item = Menu["clock"]!;

            item.BeforeValueChange += args => Clock.SetMode(args.NewValue<int>());

            Clock.SetMode(item.GetValue<int>());
        }

        private static void HandlePosition()
        {
            var menu = Menu.GetMenu("menu")!;

            var x = menu["x"]!;
            var y = menu["y"]!;

            x.BeforeValueChange += args => Menu.SetPosition(args.NewValue<int>(), y.GetValue<int>());
            y.BeforeValueChange += args => Menu.SetPosition(x.GetValue<int>(), args.NewValue<int>());

            Menu.SetPosition(x.GetValue<int>(), y.GetValue<int>());
        }

        private static void HandleTheme()
        {
            var item = Menu.Get<MenuList>("theme")!;
            var selected = item.GetValue<int>();

            if (selected == 0 && !Platform.HasTheme)
            {
                selected = 1;
                item.SetValue(selected);
            }

            Theme.SetTheme(selected);

            item.BeforeValueChange += args =>
            {
                var value = args.NewValue<int>();

                if (value == 0 && !Platform.HasTheme)
                {
                    args.Block();
                    return;
                }

                Theme.SetTheme(value);
            };

            Menu.OnLanguageChanged += args => UpdateOptions();
            UpdateOptions();

            void UpdateOptions()
            {
                var options = item.Options;
                var s = options[0].Replace("{platform}", Platform.Name);

                if (!Platform.HasTheme)
                {
                    var str = GetString("platformHasNoTheme");
                    s += $" ({str})";
                }

                options[0] = s;

                item.Options = options;
            }
        }

        private static void HandleNotifications()
        {
            var menu = Menu.GetMenu("notifications")!;

            var borders = menu["borders"]!;
            var decay = menu["decayTime"]!;

            borders.BeforeValueChange += args => Notification.SetBorders(args.NewValue<bool>());
            decay.BeforeValueChange += args => Notification.SetDecayTime(args.NewValue<float>());

            Notification.SetBorders(borders.GetValue<bool>());
            Notification.SetDecayTime(decay.GetValue<float>());
        }

        private static void HandleArrows()
        {
            var item = Menu.GetMenu("menu")!["arrows"]!;

            item.BeforeValueChange += args => Menu.SetArrows(args.NewValue<bool>());

            Menu.SetArrows(item.GetValue<bool>());
        }

        private static void SetMenuTriggers()
        {
            var menu = Menu.GetMenu("menu")!;

            var key = menu["key"]!.GetValue<Key>();
            var toggle = menu["toggle"]!.GetValue<bool>();

            Menu.SetTriggers(key, toggle);
        }

        private static JObject GetMenuTranslations()
        {
            var version = typeof(SdkSetup).Assembly.GetName().Version.ToString();
            
            var str = Resources.MainMenu;
            str = str.Replace("{version}", version);

            var mainMenu = JObject.Parse(str);
            var mode = JObject.Parse(Resources.Mode);

            foreach (var o in mainMenu["modes"].Skip(EnumCache<Language>.Values.Count)/*.Take(6)*/.Cast<JProperty>().Select(property => property.Value).OfType<JObject>())
            {
                foreach (var pair in mode)
                {
                    o.Add(pair.Key, pair.Value);
                }
            }

            return mainMenu;
        }
    }
}