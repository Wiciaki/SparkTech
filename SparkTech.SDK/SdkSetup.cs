namespace SparkTech.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.Champion;
    using SparkTech.SDK.DamageLibrary;
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.Evade;
    using SparkTech.SDK.GUI;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.GUI.Notifications;
    using SparkTech.SDK.HealthPrediction;
    using SparkTech.SDK.Input;
    using SparkTech.SDK.League;
    using SparkTech.SDK.Licensing;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.MovementPrediction;
    using SparkTech.SDK.Properties;
    using SparkTech.SDK.Rendering;
    using SparkTech.SDK.TargetSelector;
    using SparkTech.SDK.Utilities;

    public static class SdkSetup
    {
        public static readonly bool FirstRun;

        private static readonly IAuth Auth;

        private const string ValidateKey = "d1213e7b-0817-4544-aa37-01817170c494";

        private const string ProductNumber = "PUJD2J5XN";

        private static readonly Menu Menu;

        private static readonly Translations Strings;

        private readonly static Dictionary<string, Action<EventArgs>> MenuLicenseHandlers;

        private static bool authed;

        static SdkSetup()
        {
            MenuLicenseHandlers = new Dictionary<string, Action<EventArgs>>();
            Strings = new Translations { JObject.Parse(Resources.Strings) };
            Auth = new Netlicensing(Licensee.GetUserId(), ValidateKey);
            Menu = new Menu("sdk")
            {
                new Menu("modes"),
                new Menu("humanizer") 
                {
                    new MenuBool("disable", false),
                    new MenuInt("apm", 200, 600, 400)
                },
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
                new Menu("notifications")
                {
                    new MenuFloat("decayTime", 0f, 10f, 4.5f),
                    new MenuBool("borders", true),
                    new MenuAction("spawnTest", SpawnTestNotification)
                },
                new Menu("clock")
                {
                    new MenuList("mode"),
                    new MenuList("elements", Theme.WatermarkOffset == 0 ? 0 : 2),
                    new MenuBool("background", false),
                    new MenuColorBool("customColor", Color.LawnGreen, true)
                },
                new MenuList("theme"),
                new MenuList("language") { Options = EnumCache<Language>.Names },
                new Menu("loader")
                {
                    new MenuAction("refresh", Platform.ScriptLoader.Load),
                    new MenuText("loaded")
                },
                new Menu("license")
                {
                    new MenuText("core"),
                    new MenuText("sdk"),
                    new MenuAction("shop", OpenIndividualShop),
                    new MenuAction("sdkLicenseRefresh", SetSdkAuth)
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
                var texture = Texture.FromMemory(Render.Device, Resources.Banner, 295, 100, 0, Usage.None, Format.Unknown, Pool.Managed, Filter.Default, Filter.Default, 0);

                Menu.Add(new MenuTexture("banner", texture));
            }

            Mode.Initialize(Menu.GetMenu("modes"));
            Menu.Build(Menu, GetMenuTranslations());

            LoadedItem = Menu.GetMenu("loader").Get<MenuText>("loaded");
            loadedBaseText = LoadedItem.Text;

            SetupMenu(out FirstRun);

            if (Platform.HasCoreAPI)
            {
                typeof(DelayAction).Trigger();
            }

            typeof(DamageLibraryService).Trigger();
            typeof(TargetSelectorService).Trigger();
            typeof(HealthPredictionService).Trigger();
            typeof(MovementPredictionService).Trigger();
            typeof(Orbwalker.Orbwalker).Trigger();
            typeof(EvadeService).Trigger();
            typeof(UtilityService).Trigger();
            typeof(ChampionService).Trigger();

            if (!Platform.HasUserInputAPI)
            {
                Menu.SetOpen(true);

                Menu.IsExpanded = true;
                Menu.GetMenu("menu").IsExpanded = true;

                Notification.Send("This platform can't interact with the game!", "UserInputAPI not present", float.MaxValue);
            }

            Platform.ScriptLoader.Load();
        }

        internal static void SetCoreAuth(AuthResult result)
        {
            MenuAuthHelper(result, "core");
        }

        private static void SetSdkAuth()
        {
            Task.Factory.StartNew(async () =>
            {
                var result = await Auth.GetAuth(ProductNumber);
                MenuAuthHelper(result, "sdk");

                if (authed || !result.IsLicensed)
                    return;

                authed = true;
                // aaa
            });
        }

        private static void OpenIndividualShop()
        {
            Task.Factory.StartNew(async () => Process.Start(await Auth.GetShopUrl()));
        }

        private static void MenuAuthHelper(AuthResult result, string menuName)
        {
            string str;

            if (result == null || !result.IsLicensed)
            {
                str = "licenseUnlicensed";
            }
            else if (result.IsLifetime())
            {
                str = "licenseLifetime";
            }
            else
            {
                str = "licenseSubscription";
            }

            var expiry = result.Expiry.ToString(CultureInfo.InvariantCulture);
            var item = Menu.GetMenu("license").Get<MenuText>(menuName);

            void Update(EventArgs _)
            {
                Menu.ResetTranslations("license", menuName);

                var inner = GetString(str).Replace("{expiry}", expiry);
                item.Text = item.Text.Replace("{status}", "\n" + inner);
            }

            if (MenuLicenseHandlers.TryGetValue(menuName, out var value))
            {
                Menu.OnLanguageChanged -= value;
            }

            MenuLicenseHandlers[menuName] = Update;
            Menu.OnLanguageChanged += Update;

            Update(null);
        }

        private static void SetupMenu(out bool firstRun)
        {
            HandlePosition();
            HandleNotifications();
            HandleTheme();
            HandleArrows();
            HandleLoadedCount();
            HandleClock();
            HandleHumanizer();

            SetMenuTriggers();
            SetSdkAuth();

            var langItem = Menu["language"];

            langItem.BeforeValueChange += args => Menu.SetLanguage(args.NewValue<int>());

            var flagFile = Folder.Menu.GetFile(".nofirstrun");
            firstRun = !File.Exists(flagFile);

            if (!firstRun)
            {
                Menu.SetLanguage(langItem.GetValue<int>());
                return;
            }

            File.Create(flagFile).Dispose();

            Log.Info("Running SparkTech.SDK for the first time.");

            var culture = CultureInfo.InstalledUICulture;
            var languages = EnumCache<Language>.Values;
            var i = languages.ConvertAll(EnumCache<Language>.Description).IndexOf(culture.TwoLetterISOLanguageName);

            string welcomeMsg;

            if (i >= 0)
            {
                langItem.SetValue(i);

                welcomeMsg = GetString("firstTimeWelcome");
            }
            else
            {
                welcomeMsg = "Welcome to {platform}!\nYour system language,\n\"{language}\" is not supported by SparkTech.SDK.\nDefault, English will be used.\nYou can change that from within the menu.";
                welcomeMsg = welcomeMsg.Replace("{language}", culture.EnglishName);
            }

            welcomeMsg = welcomeMsg.Replace("{platform}", Platform.Name);

            Notification.Send(welcomeMsg, 10f);
        }

        internal static string GetString(string str)
        {
            return Strings.GetString(str);
        }

        private static void HandleClock()
        {
            var menu = Menu.GetMenu("clock");

            var item = menu["mode"];
            item.BeforeValueChange += args => Clock.SetMode(args.NewValue<int>());
            Clock.SetMode(item.GetValue<int>());

            item = menu["background"];
            item.BeforeValueChange += args => Clock.SetBackground(args.NewValue<bool>());
            Clock.SetBackground(item.GetValue<bool>());

            item = menu["elements"];
            item.BeforeValueChange += args => Clock.SetElements(args.NewValue<int>());
            Clock.SetElements(item.GetValue<int>());

            item = menu["customColor"];
            item.BeforeValueChange += args =>
            {
                if (args.ValueIs<bool>())
                {
                    Clock.SetCustomColor(item.GetValue<Color>(), args.NewValue<bool>());
                }
                else
                {
                    Clock.SetCustomColor(args.NewValue<Color>(), item.GetValue<bool>());
                }
            };

            Clock.SetCustomColor(item.GetValue<Color>(), item.GetValue<bool>());
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

        private static void HandleHumanizer()
        {
            var menu = Menu.GetMenu("humanizer");

            var disabled = menu["disable"];
            var apm = menu["apm"];
            apm.IsVisible = !disabled.GetValue<bool>();

            if (Platform.HasCoreAPI)
            {
                Humanizer.Apm = apm.GetValue<int>();
                Humanizer.IsEnabled = !disabled.GetValue<bool>();

                apm.BeforeValueChange += args => Humanizer.Apm = apm.GetValue<int>();
            }

            disabled.BeforeValueChange += args =>
            {
                var value = !args.NewValue<bool>();

                if (Platform.HasCoreAPI)
                {
                    Humanizer.IsEnabled = value;
                }

                apm.IsVisible = value;
            };
        }

        private static void HandleTheme()
        {
            var item = Menu.Get<MenuList>("theme");
            var selected = item.GetValue<int>();

            if (selected == 0 && !Platform.HasOwnTheme)
            {
                selected = 1;
                item.SetValue(selected);
            }

            if (Platform.HasRenderAPI)
            {
                Theme.SetTheme(selected);
            }

            item.BeforeValueChange += args =>
            {
                var value = args.NewValue<int>();

                if (value == 0 && !Platform.HasOwnTheme)
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

                if (!Platform.HasOwnTheme)
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
            var menu = Menu.GetMenu("notifications");

            var borders = menu["borders"];
            var decay = menu["decayTime"];

            borders.BeforeValueChange += args => Notification.SetBorders(args.NewValue<bool>());
            decay.BeforeValueChange += args => Notification.SetDecayTime(args.NewValue<float>());

            Notification.SetBorders(borders.GetValue<bool>());
            Notification.SetDecayTime(decay.GetValue<float>());
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
            var version = typeof(SdkSetup).Assembly.GetName().Version.ToString();
            
            var str = Resources.MainMenu;
            str = str.Replace("{version}", version);

            var mainMenu = JObject.Parse(str);
            var mode = JObject.Parse(Resources.Mode);

            foreach (var o in mainMenu["modes"].Skip(EnumCache<Language>.Values.Count).Take(6).Cast<JProperty>().Select(prop => prop.Value).OfType<JObject>())
            {
                foreach (var pair in mode)
                {
                    o.Add(pair.Key, pair.Value);
                }
            }

            return mainMenu;
        }

        private static void SpawnTestNotification()
        {
            Notification.Send("Hello, world!\nThis is an example of a lenghty notification.", "Test successful");
        }

        private static readonly MenuText LoadedItem;

        private static int loadedCount;

        private static string loadedBaseText;

        private static void HandleLoadedCount()
        {
            Menu.OnLanguageChanged += delegate
            {
                loadedBaseText = LoadedItem.Text;
                SetScriptsCount(loadedCount);
            };

            SetScriptsCount(0);
        }

        internal static void SetScriptsCount(int i)
        {
            loadedCount = i;
            LoadedItem.Text = loadedBaseText.Replace("{i}", i.ToString());
        }
    }
}