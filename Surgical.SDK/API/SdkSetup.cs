namespace Surgical.SDK.API
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
    using Surgical.SDK.Security;
    using Surgical.SDK.TargetSelector;

    public static class SdkSetup
    {
        public static readonly bool FirstRun;

        //public static readonly IAuth SurgicalAuth;

        private static readonly Menu Menu;

        private static readonly Translations Strings;

        static SdkSetup()
        {
            var texture = Texture.FromMemory(Render.Device, Resources.Banner, 295, 100, 0, Usage.None, Format.Unknown, Pool.Default, Filter.Default, Filter.Default, 0);

            Strings = new Translations();
            Strings.Set(JObject.Parse(Resources.Strings));

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
                new Menu("license")
                {
                    new MenuText("lifetime"),
                    new MenuText("valid"),
                    new MenuText("invalid")
                },
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

            typeof(HealthPredictionService).Trigger();
            typeof(MovementPredictionService).Trigger();
            typeof(TargetSelectorService).Trigger();
            typeof(OrbwalkerService).Trigger();
            typeof(EvadeService).Trigger();
            typeof(ChampionService).Trigger();

            //SurgicalAuth = new Netlicensing(Machine.UserId, "d1213e7b-0817-4544-aa37-01817170c494");
            //var AuthTask = SurgicalAuth.GetAuth("Surgical.SDK").ContinueWith(HandleAuth, TaskScheduler.Current);

            Log.Info("Surgical.SDK initialized!");
            
            // test code

            Menu.SetOpen(true);

            Menu.IsExpanded = true;
            Menu.GetMenu("menu").IsExpanded = true;
            //Menu.GetMenu("modes").GetMenu("harass").IsExpanded = true;
            //((IExpandable)Menu.GetMenu("modes").GetMenu("harass")["objects"]).IsExpanded = true;
            //((IExpandable)Menu["clock"]).IsExpanded = true;

            //Menu["clock"].SetValue(1);
            //Menu.GetMenu("modes").GetMenu("harass")["champAuto"].SetValue(false);

            //var items = Menu.OfType<IExpandable>().ToList();
            //var i = 0;

            //var timer = new System.Timers.Timer(1000d) { Enabled = true };
            //timer.Elapsed += delegate
            //{
            //    items.ForEach(item => item.IsExpanded = false);
            //    items[i++ % items.Count].IsExpanded = true;
            //};
        }

        internal static void SetupAuth(AuthResult result)
        {

        }

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

            if (i >= 0)
            {
                Menu["language"].SetValue(i);

                welcomeMsg = GetTranslatedString("firstTimeWelcome");
            }
            else
            {
                welcomeMsg = GetTranslatedString("languageUnknown");
                welcomeMsg = welcomeMsg.Replace("{language}", culture.EnglishName);
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
            var item = Menu["clock"];

            item.BeforeValueChange += args => Clock.SetMode(args.NewValue<int>());

            Clock.SetMode(item.GetValue<int>());
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

            foreach (var o in mainMenu["modes"].Skip(EnumCache<Language>.Values.Count)/*.Take(6)*/.Cast<JProperty>().Select(prop => prop.Value).OfType<JObject>())
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