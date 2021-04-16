namespace SparkTech.SDK
{
    using System;

    using SparkTech.SDK.API;
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.GUI;
    using SparkTech.SDK.Input;
    using SparkTech.SDK.League;
    using SparkTech.SDK.Licensing;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Modules;
    using SparkTech.SDK.Packets;
    using SparkTech.SDK.Rendering;

    public class Platform
    {
        public static string Name { get; private set; }

        public IRenderAPI RenderAPI { get; set; }

        public IUserInputAPI UserInputAPI { get; set; }

        public ICoreAPI CoreAPI { get; set; }

        public ITheme Theme { get; set; }

        public ILogger Logger { get; set; }

        public AuthResult AuthResult { get; set; }

        public PlatformFixes Fixes { get; set; }

        public Loader Loader { get => ScriptLoader; set => ScriptLoader = value; }

        public static bool HasRenderAPI => RenderFragment != null;

        public static bool HasCoreAPI => CoreFragment != null;

        public static bool HasUserInputAPI => UserInputFragment != null;

        public static bool HasOwnTheme => PlatformTheme != null;

        private static Platform platform;

        internal static IRenderAPI RenderFragment => platform.RenderAPI;

        internal static ICoreAPI CoreFragment => platform.CoreAPI;

        internal static IUserInputAPI UserInputFragment => platform.UserInputAPI;

        internal static ITheme PlatformTheme => platform.Theme;

        internal static ILogger PlatformLogger => platform.Logger;

        internal static Loader ScriptLoader { get; private set; } = new Loader();

        internal static bool IsLoaded { get; private set; }

        public Platform(string name)
        {
            if (Name != null)
            {
                throw new InvalidOperationException($"Platform, \"{Name}\" already declared!");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Invalid platform name", nameof(name));
            }

            Name = name;
            platform = this;
        }

        public void Load()
        {
            typeof(Log).Trigger();

            if (HasRenderAPI)
            {
                typeof(Render).Trigger();
            }
            else
            {
                Log.Warn("RenderAPI not present!");
            }

            if (HasUserInputAPI)
            {
                typeof(UserInput).Trigger();
            }
            else
            {
                Log.Warn("UserInputAPI not present!");
            }

            if (HasCoreAPI)
            {
                typeof(ObjectManager).Trigger();
                typeof(EntityEvents).Trigger();
                typeof(Humanizer).Trigger();
                typeof(Game).Trigger();
                typeof(Packet).Trigger();
            }
            else
            {
                Log.Warn("CoreAPI not present!");
            }

            // also triggers Theme class .cctor
            GUI.Theme.WatermarkOffset = this.Fixes?.WatermarkOffset ?? 0;
            SdkSetup.SetCoreAuth(this.AuthResult);

            IsLoaded = true;
            this.Fixes?.PostLoadAction?.SafeInvoke();

            HelloWorld();
        }

        private static void HelloWorld()
        {
            var message = "SparkTech.SDK initialized!";
            Log.Info(message);

            int width;

            try
            {
                width = Console.WindowWidth;
            }
            catch
            {
                return;
            }

            message = ">>>>>>> " + message + " <<<<<<<";

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(string.Format($"\n{{0,{(width + message.Length) / 2}}}\n", message));
            Console.ForegroundColor = color;
        }

        internal static Exception FragmentException()
        {
            if (!HasCoreAPI)
            {
                return APIException("CoreAPI");
            }

            return new ArgumentException("CoreAPI was provided, but one of the fragments was null");
        }

        internal static InvalidOperationException APIException(string apiName)
        {
            return new InvalidOperationException($"Attempted to use {apiName} when it wasn't present!");
        }
    }
}