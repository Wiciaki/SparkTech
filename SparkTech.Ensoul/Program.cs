namespace SparkTech.Ensoul
{
    using System.Linq;

    using SDK;
    using SDK.Licensing;
    using SDK.DamageLibrary;
    using SDK.Orbwalker;

    using Ports;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var platform = new Platform("EnsoulSharp")
            {
                AuthResult = AuthResult.GetLifetime(),
                RenderAPI = new RenderAPI(),
                UserInputAPI = new UserInputAPI(),
                CoreAPI = new CoreAPI(),
                Fixes = new PlatformFixes
                {
                    WatermarkOffset = 13,

                }
            };

            platform.Load(PostLoad);
        }

        private static void PostLoad()
        {
            Orbwalker.Picker.Add(new EnsoulOrbwalker());

            DamageLibraryService.Picker.Add(new EnsoulDamageLibrary());
            DamageLibraryService.Picker.OnModuleSelected += args => args.Block();

            EnsoulSharp.SDK.Utility.DelayAction.Add(0, () =>
            {
                SetEnsoulOrbwalker(false);

                // disable the default ensoul draws
                var menu = (EnsoulSharp.SDK.MenuUI.Menu)EnsoulSharp.SDK.MenuUI.MenuManager.Instance.Menus[2]["Drawing"];

                foreach (var item in menu.Components.Values.Cast<EnsoulSharp.SDK.MenuUI.MenuBool>())
                {
                    item.SetValue(false);
                }
            });
        }

        internal static void SetEnsoulOrbwalker(bool enable)
        {
            EnsoulSharp.SDK.Orbwalker.AttackEnabled = EnsoulSharp.SDK.Orbwalker.MoveEnabled = enable;
        }
    }
}