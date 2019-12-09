namespace ThirdPartyTest
{
    using Newtonsoft.Json.Linq;
    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.TargetSelector;
    using System.Collections.Generic;

    class Program
    {
        static void Main()
        {
            TargetSelectorService.Picker.Add(new SomeTsIdk());
        }
    }

    class SomeTsIdk : ITargetSelector
    {
        public Menu Menu { get; } = new Menu("Works?");

        public IHero GetTarget(IEnumerable<IHero> targets)
        {
            return null;
        }

        public JObject GetTranslations()
        {
            return null;
        }

        public void Pause()
        {

        }

        public void Start()
        {

        }
    }

    //using System.Collections.Generic;
    //using System.Linq;

    //using Surgical.SDK.API;
    //using Surgical.SDK.Logging;
    //using Surgical.SDK.Modules;

    //public class SampleUtility : IScript
    //{
    //    public IEnumerable<IModule> GetModules()
    //    {
    //        return Enumerable.Empty<IModule>();
    //    }

    //    public SampleUtility()
    //    {
    //        Log.Info("SampleUtility loaded!");

    //        Log.Info("Platform name " + Platform.PlatformName);
    //    }
    //}
}