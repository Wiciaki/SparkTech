namespace Surgical.SDK.Utilities
{
    using Newtonsoft.Json.Linq;

    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.Properties;

    public static class UtilityService
    {
        static UtilityService()
        {
            var utilities = new IUtility[] { new Activator(), new DamageIndicator() };

            var active = new Menu("active");
            var menu = new Menu("utilities") { active };

            foreach (var utility in utilities)
            {
                active.Add(new MenuBool(utility.Menu.Id, true));
                menu.Add(utility.Menu);
            }

            Menu.Build(menu, JObject.Parse(Resources.Utilities));

            foreach (var utility in utilities)
            {
                var item = active[utility.Menu.Id]!;

                if (item.GetValue<bool>())
                {
                    utility.Start();
                }
                else
                {
                    utility.Menu.IsVisible = false;
                }

                item.BeforeValueChange += args =>
                {
                    if (args.NewValue<bool>())
                    {
                        utility.Start();
                        utility.Menu.IsVisible = true;
                    }
                    else
                    {
                        utility.Menu.IsVisible = false;
                        utility.Pause();
                    }
                };
            }
        }
    }
}