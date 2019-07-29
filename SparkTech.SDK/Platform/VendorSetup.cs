namespace SparkTech.SDK.Platform
{
    using System.Reflection;

    using SparkTech.SDK.Auth;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Logging.Default;
    using SparkTech.SDK.UI;

    public sealed class VendorSetup
    {
        private VendorSetup()
        { }

        private static bool flag;

        public static VendorSetup GetTrustedInstance()
        {
            if (flag)
            {
                return null;
            }

            if (VendorValidation.IsTrusted(Assembly.GetCallingAssembly()))
            {
                flag = true;

                return new VendorSetup();
            }

            return null;
        }

        internal static IPlatform Platform { get; private set; }

        public void SetPlatform(IPlatform platform)
        {
            Platform = platform;
        }

        internal static ITheme Theme { get; private set; }

        public static void SetDefaultPlatformTheme(ITheme theme)
        {
            Theme = theme;
        }

        internal static IAuth Auth { get; private set; }

        public static void SetAuth(IAuth auth)
        {
            Auth = auth;
        }

        internal static ILogger Logger { get; private set; } = new DefaultLogger();

        public static void SetLogger(ILogger logger)
        {
            Logger = logger;
        }
    }
}