namespace SurgicalSample
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.GUI.Notifications;
    using Surgical.SDK.TargetSelector;

    using SurgicalSample.Properties;

    // see notes in Program.cs

    // ReSharper disable once UnusedMember.Global
    public class TargetSelector : ITargetSelector
    {
        // in modules, you don't need to call Build(...) at all!
        public Menu Menu { get; } = new Menu("sampleTS")
                                    {
                                        new MenuText("sampleItem")
                                    };

        public JObject GetTranslations()
        {
            // one small note here
            // you can do it in a different way, as a byte stream, too
            // but here I've added the .json to resources, changed its type to "text" and set encoding in properties to Windows-1250
            return JObject.Parse(Resources.TS);
        }

        // will be executes when user picks it from module list..
        public void Start()
        {
            Notification.Send("You've chosen my Target Selector <3", "Yaay!");
        }

        // will be chosen when user picks another TS, later...
        public void Pause()
        {
            Notification.Send("~TS team", "Byee :(");
        }

        // the most useful method here - serves actual TS purpose
        public IHero GetTarget(IEnumerable<IHero> targets)
        {
            // braindead logic
            // attacks enemies alphabetically by champion name, not too useful, 2/10
            return targets.OrderByDescending(hero => hero.CharName).FirstOrDefault();
        }
    }
}