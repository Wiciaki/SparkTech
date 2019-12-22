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
        public static string Name { get; private set; }

        public static bool HasRenderAPI { get; private set; }

        public static bool HasCoreAPI { get; private set; }

        public static bool HasUserInputAPI { get; private set; }

        public IRenderAPI RenderAPI { get; set; }

        public ICoreAPI CoreAPI { get; set; }
        
        public IUserInputAPI UserInputAPI { get; set; }

        public AuthResult AuthResult { get; set; }

        public ITheme Theme { get; set; }

        public IScriptLoader ScriptLoader { get; set; }

        public ILogger Logger { get; set; }

        public void Boot()
        {
            Log.SetLogger(this.Logger ??= new FileLogger());

            if (this.RenderAPI != null)
            {
                HasRenderAPI = true;

                Render.Initialize(this.RenderAPI);
            }
            else
            {
                Log.Warn("RenderAPI not present!");
            }

            if (this.UserInputAPI != null)
            {
                HasUserInputAPI = true;

                UserInput.Initialize(this.UserInputAPI);
            }
            else
            {
                Log.Warn("UserInputAPI not present!");
            }

            if (this.CoreAPI != null)
            {
                HasCoreAPI = true;

                ObjectManager.Initialize(this.CoreAPI.GetObjectManagerFragment() ?? throw InvalidAPI());
                EntityEvents.Initialize(this.CoreAPI.GetEntityEventsFragment() ?? throw InvalidAPI());
                Player.Initialize(this.CoreAPI.GetPlayerFragment() ?? throw InvalidAPI());
                Game.Initialize(this.CoreAPI.GetGameFragment() ?? throw InvalidAPI());
                Packet.Initialize(this.CoreAPI.GetPacketFragment() ?? throw InvalidAPI());

                static ArgumentException InvalidAPI() => new ArgumentException("CoreAPI was provided, but one of the fragments was null");
            }
            else
            {
                Log.Warn("CoreAPI not present!");
            }

            GUI.Theme.Initialize(this.Theme);
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
    }
}