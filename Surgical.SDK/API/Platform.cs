namespace Surgical.SDK.API
{
    using System;
    using System.Collections.Generic;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.GUI;
    using Surgical.SDK.Licensing;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Modules;
    using Surgical.SDK.Security;

    public sealed class Platform
    {
        public static string PlatformName { get; private set; }

        internal static List<IModule> Modules { get; private set; }

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

        public AuthResult AuthResult { get; set; }

        public ILogger Logger { get; set; }

        public ITheme Theme { get; set; }

        public IRender Render { get; set; }

        public IObjectManager ObjectManager { get; set; }

        public IPlayer Player { get; set; }

        public IEntityEvents EntityEvents { get; set; }

        public IGame Game { get; set; }

        public IPacket Packet { get; set; }

        public ISandbox Sandbox { get; set; }

        public string FolderPath { get; set; }

        public void Boot()
        {
            if (this.FolderPath == null)
            {
                var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var folder = new Folder(appdata);

                this.FolderPath = folder.GetFolder("Surgical.SDK");
            }

            Folder.Initialize(this.FolderPath);

            if (this.Logger == null)
            {
                this.Logger = new FileLogger();
            }

            Log.Initialize(this.Logger);

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

            if (this.Theme == null)
            {
                this.Theme = new SurgicalTheme();
            }

            GUI.Theme.SetTheme(this.Theme);
            SdkSetup.SetupAuth(this.AuthResult);

            if (this.Sandbox == null)
            {
                this.Sandbox = new Sandbox();
            }

            this.Sandbox.LoadScripts();
        }
    }
}