namespace SparkTech.SDK.Platform
{
    using System.Reflection;

    using SparkTech.SDK.Auth;
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.GUI;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Rendering;
    using SparkTech.SDK.Security;

    public sealed class VendorSetup
    {
        public static string PlatformName { get; private set; }

        private VendorSetup()
        { }

        private static int state;

        public static VendorSetup GetTrustedInstance()
        {
            if (state != 0)
            {
                return null;
            }

            if (!VendorValidation.IsTrusted(Assembly.GetCallingAssembly()))
            {
                return null;
            }

            state++;

            return new VendorSetup();
        }

        public void Boot(IPlatform platform)
        {
            //if (state )

            ObjectManager.Initialize(platform.GetObjectManager());

            Render.Initialize(platform.GetRender());

            Spellbook.Initialize(platform.GetSpellbook());
        }

        public void SetDefaultPlatformTheme(ITheme theme)
        {
            // Theme = theme;
        }

        public IAuth Auth { get; set; }

        public ILogger Logger
        {
            get => Log.Logger;
            set
            {
                if (state == 1 && value != null)
                {
                    Log.Logger = value;
                }
            } 
        }
    }
}