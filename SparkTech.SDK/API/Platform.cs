﻿namespace SparkTech.SDK.API
{
    using System;
    using System.Runtime.CompilerServices;

    using SparkTech.SDK.API.Fragments;
    using SparkTech.SDK.GUI;
    using SparkTech.SDK.Licensing;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Security;

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

        public IEntityEvents EntityEvents { get; set; }

        public IGame Game { get; set; }

        public IAuth Auth { get; set; }

        //public IObjectManager ObjectManager { get; set; }

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

            if (this.Game != null)
            {
                SDK.Game.Initialize(this.Game);
            }

            GUI.Theme.SetTheme(this.Theme ?? new GUI.Default.Theme());

            RuntimeHelpers.RunClassConstructor(typeof(SdkSetup).TypeHandle);

            if (this.Auth != null)
            {
                var authResult = await this.Auth.Auth("Surgical.SDK");


            }

            // load scripts

            Sandbox.LoadThirdParty();
        }
    }
}