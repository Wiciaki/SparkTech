namespace SparkTech.SDK.API
{
    using System;
    using System.Runtime.CompilerServices;

    using SparkTech.SDK.API.Fragments;
    using SparkTech.SDK.GUI;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Security;

    public sealed class Platform
    {
        public static string PlatformName { get; private set; }

        public Platform(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Empty platform name", nameof(name));
            }

            this.Name = name;
        }

        public string Name { get; }

        public ILogger Logger { get; set; }

        public ITheme Theme { get; set; }

        public IRender Render { get; set; }

        public IObjectManager ObjectManager { get; set; }

        public IEntityEvents EntityEvents { get; set; }

        //public IObjectManager ObjectManager { get; set; }

        public string ConfigPath { get; set; }

        public void Boot()
        {
            PlatformName = this.Name;

            if (this.ConfigPath == null)
            {
                var folder = new Folder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

                this.ConfigPath = folder.GetFolder("Surgical.SDK");
            }

            Folder.Initialize(this.ConfigPath);
            Log.Initialize(this.Logger ?? new FileLogger());

            if (this.Render != null)
            {
                Rendering.Render.Initialize(this.Render);
            }

            if (this.ObjectManager != null)
            {
                Entities.ObjectManager.Initialize(this.ObjectManager);
            }

            if (this.EntityEvents != null)
            {
                Entities.Events.EntityEvents.Initialize(this.EntityEvents);
            }

            GUI.Theme.SetTheme(this.Theme ?? new GUI.Default.Theme());

            RuntimeHelpers.RunClassConstructor(typeof(SdkSetup).TypeHandle);
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