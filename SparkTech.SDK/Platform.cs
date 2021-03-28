namespace SparkTech.SDK
{
    using System;
    using System.Linq;

    using SparkTech.SDK.API;
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.GUI;
    using SparkTech.SDK.Licensing;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Modules;
    using SparkTech.SDK.Packets;
    using SparkTech.SDK.Rendering;

    using SparkTech.SDK.TargetSelector;

    public sealed class Platform
    {
        public static string Name { get; private set; }

        internal static IRenderAPI RenderFragment { get; private set; }

        internal static ICoreAPI CoreFragment { get; private set; }
        
        internal static IUserInputAPI UserInputFragment { get; private set; }

        internal static ITheme PlatformTheme { get; private set; }

        internal static ILogger PlatformLogger { get; private set; }

        internal static Loader ScriptLoader { get; private set; } = new Loader();

        public static bool HasRenderAPI => RenderFragment != null;

        public static bool HasCoreAPI => CoreFragment != null;

        public static bool HasUserInputAPI => UserInputFragment != null;

        public static bool HasOwnTheme => PlatformTheme != null;

        public IRenderAPI RenderAPI { get; set; }

        public ICoreAPI CoreAPI { get; set; }
        
        public IUserInputAPI UserInputAPI { get; set; }

        public ITheme Theme { get; set; }

        public ILogger Logger { get; set; }

        public AuthResult AuthResult { get; set; }

        public Loader Loader { get => ScriptLoader; set => ScriptLoader = value; }

        public int WatermarkOffset { get; set; }

        public void Load()
        {
            RenderFragment = this.RenderAPI;
            UserInputFragment = this.UserInputAPI;
            CoreFragment = this.CoreAPI;

            PlatformLogger = this.Logger;
            PlatformTheme = this.Theme;

            ScriptLoader = this.Loader;

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

            GUI.Theme.WatermarkOffset = this.WatermarkOffset; // also triggers Theme class .cctor

            SdkSetup.SetAuth(this.AuthResult);

            Log.Info("SparkTech.SDK initialized!");

            Render.OnDraw += delegate
                {
                    //var hero = ObjectManager.Get<IHero>().Where(o => o.Distance(ObjectManager.Player) < 550f).GetTarget();

                    //if (hero != null)
                    //    Circle.Draw(SharpDX.Color.Magenta, hero.BoundingRadius, 1f, true, hero.Position);

                    //foreach (var o in ObjectManager.Get<IUnit>())
                    //    Text.Draw(o.Name, SharpDX.Color.Magenta, (SharpDX.Point)Game.WorldToScreen(o.Position));

                };
        }

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