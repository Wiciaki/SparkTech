namespace ThirdPartyTest
{
    using System.Collections.Generic;
    using System.Linq;

    using Surgical.SDK.API;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Modules;

    public class SampleUtility : IScript
    {
        public IEnumerable<IModule> GetModules()
        {
            return Enumerable.Empty<IModule>();
        }

        public SampleUtility()
        {
            Log.Info("SampleUtility loaded!");

            Log.Info("Platform name " + Platform.PlatformName);
        }
    }
}