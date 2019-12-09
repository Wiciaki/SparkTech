namespace Surgical.SDK.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json.Linq;

    using Surgical.SDK.EventData;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.Properties;
    using Surgical.SDK.Security;

    public sealed class Picker<TModule> where TModule : class, IModule
    {
        private readonly List<TModule> modules;

        private readonly Folder folder;

        private readonly Menu root;

        private readonly MenuList picker;

        internal TModule Current { get; private set; }

        public event Action<BeforeValueChangeEventArgs> OnModuleSelected;

        public void Add(TModule module)
        {
            this.modules.Add(module);

            this.root.Add(module.Menu, module.GetTranslations());
            module.Menu.CreateSaveHandler(this.folder);

            this.UpdateOptions();

            this.picker.IsVisible = true;
            this.root.IsVisible = true;
        }

        internal Picker(TModule module)
        {
            this.modules = new List<TModule> { module };

            var input = typeof(TModule).Name;

            if (Regex.IsMatch(input, "^I[A-Z]"))
            {
                input = input.Substring(1);
            }

            this.picker = new MenuList("picker") { IsVisible = false, IsExpanded = true, Options = { module.Menu.Text } };
            this.picker.BeforeValueChange += this.BeforeValueChange;

#pragma warning disable CA1304 // Specify CultureInfo
            this.root = new Menu(input.ToLower()) { IsVisible = module.Menu.Any(), IsExpanded = true }; //
#pragma warning restore CA1304 // Specify CultureInfo
            this.root.Add(this.picker);
            this.root.Add(module.Menu);

            this.folder = Folder.Menu.GetFolder(input);
            module.Menu.CreateSaveHandler(this.folder);

            Menu.Build(this.root, GetTranslations(input), false);
            Menu.OnLanguageChanged += delegate { this.UpdateOptions(); };

            this.Current = module;
            this.Current.Start();
        }

        private static JObject GetTranslations(string input)
        {
            var name = Regex.Replace(input, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

            return JObject.Parse(Resources.Module.Replace("{module}", name, StringComparison.InvariantCulture));
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

            this.Current.Pause();
            this.Current = module;
            this.Current.Start();
        }

        private void UpdateOptions()
        {
            this.picker.Options = this.modules.ConvertAll(m => m.Menu.Text);
        }
    }
}