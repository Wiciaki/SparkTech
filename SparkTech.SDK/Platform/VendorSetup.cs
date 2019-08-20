namespace SparkTech.SDK.Platform
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using SharpDX;

    using SparkTech.SDK.Auth;
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.Game;
    using SparkTech.SDK.GUI;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Platform.API;
    using SparkTech.SDK.Rendering;
    using SparkTech.SDK.Security;

    public sealed class VendorSetup
    {
        public static string PlatformName { get; private set; }

        public static void Init<T>(string platName, T thing) where T : IRender, IGameEvents
        {
            PlatformName = platName;

            var desktop = new Folder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            Folder.Initialize(desktop.GetFolder("Shark"));
            Log.Info("start...");

            Render.Initialize(thing);
            GameEvents.Initialize(thing);

            //Render.OnDraw += () => Vector.Draw(Color.White, 50f, new Vector2(100, 100), new Vector2(150, 150));

            RuntimeHelpers.RunClassConstructor(typeof(SdkSetup).TypeHandle);

            Clock.UpdateSize();

            Menu.UpdateAllSizes();
        }

        /*
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
        }*/
    }
}