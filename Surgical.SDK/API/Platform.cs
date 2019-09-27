namespace Surgical.SDK.API
{
    using System;
    using System.Runtime.CompilerServices;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.GUI;
    using Surgical.SDK.Licensing;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Security;

    public sealed class Platform
    {
        public static string PlatformName { get; private set; }

        public static Platform Declare(string platformName)
        {
            if (string.IsNullOrWhiteSpace(platformName))
            {
                throw new ArgumentException("Empty platform name", nameof(platformName));
            }

            // verify here

            PlatformName = platformName;

            return new Platform();
        }

        private Platform()
        {

        }

        public ILogger Logger { get; set; }

        public ITheme Theme { get; set; }

        public IRender Render { get; set; }

        public IObjectManager ObjectManager { get; set; }

        public IPlayer Player { get; set; }

        public IEntityEvents EntityEvents { get; set; }

        public IGame Game { get; set; }

        public IPacket Packet { get; set; }

        public ISandbox Sandbox { get; set; }

        public string ConfigPath { get; set; }

        public async void Boot()
        {
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
                Entities.EntityEvents.Initialize(this.EntityEvents);
            }

            if (this.Player != null)
            {
                Entities.Player.Initialize(this.Player);
            }

            if (this.Game != null)
            {
                SDK.Game.Initialize(this.Game);
            }

            if (this.Packet != null)
            {
                Packets.Packet.Initialize(this.Packet);
            }

            GUI.Theme.SetTheme(this.Theme ?? new SurgicalTheme());

            RuntimeHelpers.RunClassConstructor(typeof(SdkSetup).TypeHandle);

            var auth = new Netlicensing(Machine.UserId, "d1213e7b-0817-4544-aa37-01817170c494");
            //var authResult = await auth.GetAuth("Surgical.SDK");

            //Log.Info(authResult);

            // load scripts

            Security.Sandbox.Initialize(this.Sandbox);
        }
    }
}