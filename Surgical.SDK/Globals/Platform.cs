namespace Surgical.SDK
{
    using System;

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
        public static string? Name { get; private set; }

        internal static IRenderAPI? RenderFragment { get; private set; }

        internal static ICoreAPI? CoreFragment { get; private set; }
        
        internal static IUserInputAPI? UserInputFragment { get; private set; }

        internal static ITheme? PlatformTheme { get; private set; }

        internal static ILogger? PlatformLogger { get; private set; }

        internal static readonly Loader ScriptLoader = new Loader();

        public static bool HasRenderAPI => RenderFragment != null;

        public static bool HasCoreAPI => CoreFragment != null;

        public static bool HasUserInputAPI => UserInputFragment != null;

        public static bool HasOwnTheme => PlatformTheme != null;

        public IRenderAPI? RenderAPI { get; set; }

        public ICoreAPI? CoreAPI { get; set; }
        
        public IUserInputAPI? UserInputAPI { get; set; }

        public ITheme? Theme { get; set; }

        public Loader Loader { get; }

        public ILogger? Logger { get; set; }

        public AuthResult? AuthResult { get; set; }

        public void Boot()
        {
            RenderFragment = this.RenderAPI;
            UserInputFragment = this.UserInputAPI;
            CoreFragment = this.CoreAPI;

            PlatformLogger = this.Logger;
            PlatformTheme = this.Theme;

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
                typeof(Player).Trigger();
                typeof(Game).Trigger();
                typeof(Packet).Trigger();
            }
            else
            {
                Log.Warn("CoreAPI not present!");
            }

            typeof(Theme).Trigger();

            SdkSetup.SetAuth(this.AuthResult);
        }

        public static Platform Declare(string name)
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

            return new Platform();
        }

        private Platform()
        {
            this.Loader = ScriptLoader;
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