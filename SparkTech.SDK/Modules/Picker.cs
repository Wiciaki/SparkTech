namespace SparkTech.SDK.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.EventData;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Properties;

    public sealed class Picker<TModule> where TModule : class, IModule
    {
        public event Action<BeforeValueChangeEventArgs> OnModuleSelected;

        private readonly List<TModule> modules;

        private readonly Folder folder;

        private readonly Menu root;

        private readonly MenuList picker;

        internal TModule Current { get; private set; }

        public void Add(TModule module)
        {
            this.modules.Add(module);

            this.root.Add(module.Menu, module.GetTranslations());
            module.Menu.CreateSaveHandler(this.folder);

            this.UpdateOptions();

            this.picker.IsVisible = true;
            this.root.IsVisible = true;

            if (typeof(TModule).Name == "IDamageLibrary") this.picker.SetValue(1); // temp fix
        }

        internal Picker(TModule module)
        {
            this.modules = new List<TModule> { module };

            var input = typeof(TModule).Name;

            if (Regex.IsMatch(input, "^I[A-Z]"))
            {
                input = input.Substring(1);
            }

            this.picker = new MenuList("picker") { IsVisible = false, Options = new List<string> { module.Menu.Text } };
            this.picker.BeforeValueChange += this.BeforeValueChange;

            this.root = new Menu(input.ToLower()) { IsVisible = module.Menu.Any() };
            this.root.Add(this.picker);
            this.root.Add(module.Menu, module.GetTranslations());

            this.folder = Folder.Menu.GetFolder(input);
            module.Menu.CreateSaveHandler(this.folder);

            Menu.Build(this.root, GetTranslations(input), false);
            Menu.OnLanguageChanged += args => this.UpdateOptions();

            if (Platform.HasCoreAPI)
            {
                module.Start();
            }

            this.Current = module;
        }

        private static JObject GetTranslations(string input)
        {
            var name = Regex.Replace(input, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

            return JObject.Parse(Resources.Module.Replace("{module}", name));
        }

        private void BeforeValueChange(BeforeValueChangeEventArgs args)
        {
            var module = this.modules[args.NewValue<int>()];

            var oldValue = this.Current.Menu.Id;
            var newValue = module.Menu.Id;

            var detector = BeforeValueChangeEventArgs.Create(oldValue, newValue);
            this.OnModuleSelected.SafeInvoke(detector);

            if (detector.IsBlocked)
            {
                args.Block();
                return;
            }

            if (Platform.HasCoreAPI)
            {
                this.Current.Pause();
                module.Start();
            }

            this.Current = module;
        }

        private void UpdateOptions()
        {
            this.picker.Options = this.modules.ConvertAll(m => m.Menu.Text);
        }
    }
}