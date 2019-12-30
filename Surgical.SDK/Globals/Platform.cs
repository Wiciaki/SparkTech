namespace Surgical.SDK
{
    using System;
    using System.IO;

    using Surgical.SDK.API;
    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI;
    using Surgical.SDK.Licensing;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Modules;
    using Surgical.SDK.Packets;
    using Surgical.SDK.Rendering;

    public sealed class Platform
    {
        public static string Name { get; private set; } = "";

        public static bool HasRenderAPI { get; private set; }

        internal static IRenderAPI? RenderFragment;

        public static bool HasCoreAPI { get; private set; }

        internal static ICoreAPI? CoreFragment;

        public static bool HasUserInputAPI { get; private set; }

        internal static IUserInputAPI? UserInputFragment;

        public static bool HasTheme { get; private set; }

        internal static ITheme? PlatformTheme;

        public IRenderAPI? RenderAPI { get; set; }

        public ICoreAPI? CoreAPI { get; set; }
        
        public IUserInputAPI? UserInputAPI { get; set; }

        public AuthResult AuthResult { get; set; }

        public ITheme? Theme { get; set; }

        public IScriptLoader? ScriptLoader { get; set; }

        public ILogger? Logger { get; set; }

        internal static ILogger PlatformLogger;

        public void Boot()
        {
            typeof(Log).Trigger();

            if (this.RenderAPI != null)
            {
                HasRenderAPI = true;

                RenderFragment = this.RenderAPI;
                typeof(Render).Trigger();
            }
            else
            {
                Log.Warn("RenderAPI not present!");
            }

            if (this.UserInputAPI != null)
            {
                HasUserInputAPI = true;

                UserInputFragment = this.UserInputAPI;
                typeof(UserInput).Trigger();
            }
            else
            {
                Log.Warn("UserInputAPI not present!");
            }

            if (this.CoreAPI != null)
            {
                HasCoreAPI = true;

                CoreFragment = this.CoreAPI;
                typeof(ObjectManager).Trigger();
                typeof(EntityEvents).Trigger();
                typeof(Player).Trigger();
                typeof(Game).Trigger();
                typeof(Packet).Trigger();
            }
            else
            {
                Log.Warn("CoreAPI not present!");
            }

            HasTheme = this.Theme != null;
            PlatformTheme = this.Theme;
            typeof(Theme).Trigger();

            SdkSetup.SetupAuth(this.AuthResult);

            if (this.ScriptLoader == null)
            {
                this.ScriptLoader = new ScriptLoader();
            }

            var dir = Path.Combine(Folder.Root, Name);

            if (Directory.Exists(dir))
            {
                this.ScriptLoader.LoadFrom(dir);
            }

            this.ScriptLoader.LoadFrom(Folder.Scripts);
        }

        public static Platform Declare(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Invalid platform name", nameof(name));
            }

            Name = name;

            return new Platform();
        }

        private Platform()
        { }

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