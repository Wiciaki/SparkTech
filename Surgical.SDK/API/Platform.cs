namespace Surgical.SDK.API
{
    using System;

    using Surgical.SDK.GUI;
    using Surgical.SDK.Licensing;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Modules;

    public sealed class Platform
    {
        public static string Name { get; private set; }

        public static bool HasRender { get; private set; }

        public static bool HasAPI { get; private set; }

        public AuthResult AuthResult { get; set; }

        public IRender Render { get; set; }

        public ICoreAPI CoreAPI { get; set; }

        public ITheme Theme { get; set; }

        public IScriptLoader ScriptLoader { get; set; }

        public ILogger Logger { get; set; }

        public void Boot()
        {
            Log.SetLogger(this.Logger ??= new FileLogger());

            if (this.Render != null)
            {
                HasRender = true;

                Rendering.Render.Initialize(this.Render);
            }
            else
            {
                Log.Warn("Render not present!");
            }

            if (this.CoreAPI != null)
            {
                HasAPI = true;

                Entities.ObjectManager.Initialize(this.CoreAPI.GetObjectManagerFragment() ?? throw InvalidAPI());
                Entities.EntityEvents.Initialize(this.CoreAPI.GetEntityEventsFragment() ?? throw InvalidAPI());
                Entities.Player.Initialize(this.CoreAPI.GetPlayerFragment() ?? throw InvalidAPI());
                Game.Initialize(this.CoreAPI.GetGameFragment() ?? throw InvalidAPI());
                Packets.Packet.Initialize(this.CoreAPI.GetPacketFragment() ?? throw InvalidAPI());

                static ArgumentException InvalidAPI() => new ArgumentException("CoreAPI was provided, but one of the fragments was null");
            }
            else
            {
                Log.Warn("API not present!");
            }

            GUI.Theme.SetTheme(this.Theme ??= new SurgicalTheme());

            SdkSetup.SetupAuth(this.AuthResult);

            if (this.ScriptLoader == null)
            {
                this.ScriptLoader = new ScriptLoader();
            }

            this.ScriptLoader.LoadFrom(Folder.Scripts);
        }

        public static Platform Declare(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Invalid platform name", nameof(name));
            }

            // verify here

            Name = name;

            return new Platform();
        }

        private Platform()
        { }
    }
}