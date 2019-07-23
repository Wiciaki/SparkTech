//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  Menu.cs is a part of SparkTech
//
//  SparkTech is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  SparkTech is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  You should have received a copy of the GNU General Public License
//  along with SparkTech. If not, see <http://www.gnu.org/licenses/>.
//
//  -------------------------------------------------------------------

namespace SparkTech.UI.Menu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using SparkTech.TickOperations;
    using SparkTech.Utils;

    using Newtonsoft.Json.Linq;

    using SparkTech.Link;

    public class Menu : NamedComponent, IEnumerable<MenuComponent>
    {
        #region Static Fields

        protected static string SaveDirectory => GameEnvironment.SaveDirectory;

        #endregion

        #region Fields

        public IReadOnlyList<MenuComponent> Components { get; private set; }

        private readonly List<MenuComponent> subComponents = new List<MenuComponent>();

        protected JObject Settings;

        private JToken translations;

        private readonly Dictionary<string, Menu> childrenDict = new Dictionary<string, Menu>();

        private readonly Dictionary<string, MenuValue> valuesDict = new Dictionary<string, MenuValue>();

        #endregion

        #region Constructors and Destructors

        static Menu()
        {
            GameLoading.OnLoad += () => Game.OnWndProc += WndProc;
        }

        public Menu(string id) : base(id)
        {
            this.Components = this.subComponents.AsReadOnly();
        }

        #endregion

        #region Public Events

        public static event Action LanguageChanged, VisibilityStateChanged;

        #endregion

        #region Public Properties

        public static bool IsOpen { get; private set; }

        public static Language Language { get; private set; }

        public static WindowMessageWParam ActivationButton { get; private set; }

        #endregion

        protected internal override bool IsExpanding => true;

        #region Public Indexers

        public MenuValue this[string id] => this.valuesDict[id];

        #endregion

        #region Public Methods and Operators

        public virtual TMenuComponent Add<TMenuComponent>(TMenuComponent item) where TMenuComponent : MenuComponent
        {
            var id = item.Id;

            if (this.Components.Any(c => c.Id == item.Id))
            {
                throw new InvalidOperationException($"The provided MenuComponent key \"{id}\" is duplicate, and not unique for this menu instance!");
            }

            switch (item)
            {
                case MenuValue i:
                    this.valuesDict.Add(id, i);
                    break;
                case RootMenu _:
                    throw new InvalidOperationException("Can't add RootMenu to any other MenuComponent!");
                case Menu i:
                    this.childrenDict.Add(id, i);
                    break;
            }

            if (this.Settings != null)
            {
                item.SetToken(this.Settings[id]);
            }

            if (this.translations != null && item is NamedComponent component)
            {
                component.UpdateTranslations(this.translations[component.Id]);
            }
            else
            {
                item.UpdateSize();
            }

            this.subComponents.Add(item);

            this.Components = this.subComponents.AsReadOnly();

            return item;
        }

        public IEnumerable<MenuComponent> EnumerateDescensants()
        {
            foreach (var component in this)
            {
                yield return component;
            }

            foreach (var menu in this.childrenDict.Values)
            {
                foreach (var component in menu.EnumerateDescensants())
                {
                    yield return component;
                }
            }
        }

        public IReadOnlyCollection<Menu> EnumerateMenus()
        {
            return this.childrenDict.Values;
        }

        public IReadOnlyCollection<MenuValue> EnumerateValues()
        {
            return this.valuesDict.Values;
        }

        public Menu GetMenu(string id)
        {
            return this.childrenDict[id];
        }

        public List<MenuComponent> ListDescensants()
        {
            var components = this.ToList();

            foreach (var menu in this.childrenDict.Values)
            {
                components.AddRange(menu.ListDescensants());
            }

            return components;
        }

        public void Remove(MenuComponent component) => this.Remove(component.Id);

        public void Remove(string id)
        {
            var index = this.subComponents.FindIndex(c => c.Id == id);

            if (index == -1)
            {
                return;
            }

            this.subComponents.RemoveAt(index);

            this.Components = this.subComponents.AsReadOnly();

            if (this.valuesDict.Remove(id))
            {
                return;
            }

            this.childrenDict.Remove(id);
        }

        #endregion

        #region Explicit Interface Methods

        IEnumerator<MenuComponent> IEnumerable<MenuComponent>.GetEnumerator()
        {
            return this.subComponents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var enumerable = (IEnumerable<MenuComponent>)this;

            return enumerable.GetEnumerator();
        }

        #endregion

        #region Methods

        protected internal override JObject GetRequiredTranslations()
        {
            var @base = base.GetRequiredTranslations();

            foreach (var component in this.OfType<NamedComponent>())
            {
                var t = component.GetRequiredTranslations();

                if (t != null)
                {
                    @base.Add(component.Id, t);
                }
            }

            return @base;
        }

        protected internal override void UpdateTranslations(JToken token)
        {
            if (JToken.DeepEquals(this.translations, token))
            {
                return;
            }

            base.UpdateTranslations(token);

            foreach (var component in this.OfType<NamedComponent>()) // TODO
            {
                component.UpdateTranslations(token?[component.Id]);
            }

            this.translations = token;
        }

        private Size arrowSize;

        protected override Size GetSize()
        {
            var size = base.GetSize();

            this.arrowSize = new Size(size.Height, size.Height);

            size.Width += this.arrowSize.Width;

            return size;
        }

        protected internal override void OnWndProc(Point point, Size size, GameWndProcEventArgs args)
        {
            if (!this.IsHighlighted)
            {
                return;
            }

            var visible = this.subComponents.FindAll(c => c.IsVisible);

            if (visible.Count == 0)
            {
                return;
            }

            point.X += size.Width + Theme.ItemDistance.Width;
            size.Width = visible.Max(c => c.Size.Width);

            var check = args.Message.IsLeftClick();

            visible.ForEach(WndProc);

            void WndProc(MenuComponent component)
            {
                size.Height = component.Size.Height;

                component.OnWndProc(point, size, args);

                if (component.IsExpanding)
                {
                    if (check && Inside())
                    {
                        check = false;

                        foreach (var c in this)
                        {
                            c.IsHighlighted = c.Id == component.Id && !c.IsHighlighted;
                        }
                    }
                }
                else
                {
                    component.IsHighlighted = !this.Any(c => c.IsHighlighted && c.Id != component.Id) && Inside();
                }

                point.Y += component.Size.Height + Theme.ItemDistance.Height;

                bool Inside() => Mouse.IsInside(point, size);
            }
        }

        private void DrawSelf(Point point, Size size)
        {
            size.Width -= this.arrowSize.Width;

            base.OnEndScene(point, size);

            point.X += size.Width;

            Theme.Draw(new DrawData(point, this.arrowSize) { Text = ">>", ForceTextCentered = true });
        }

        protected internal override void OnEndScene(Point point, Size size)
        {
            this.DrawSelf(point, size);

            if (!this.IsHighlighted)
            {
                return;
            }

            var visible = this.subComponents.FindAll(c => c.IsVisible);

            if (visible.Count == 0)
            {
                return;
            }

            point.X += size.Width + Theme.ItemDistance.Width;
            size.Width = visible.Max(c => c.Size.Width);

            visible.ForEach(EndScene);

            void EndScene(MenuComponent component)
            {
                size.Height = component.Size.Height;

                component.OnEndScene(point, size);
                component.OnDrawOverlay(point, size);

                point.Y += component.Size.Height + Theme.ItemDistance.Height;
            }
        }

        protected internal override bool ShouldSave()
        {
            var found = false;

            foreach (var component in this)
            {
                if (component.ShouldSave())
                {
                    found = true;
                }
            }

            return found;
        }

        protected internal override JToken GetToken()
        {
            var itemsToAdd = new Dictionary<string, JToken>();

            foreach (var component in this)
            {
                var token = component.GetToken();

                if (token != null)
                {
                    itemsToAdd.Add(component.Id, token);
                }
            }

            if (itemsToAdd.Count == 0)
            {
                return null;
            }

            var o = new JObject();

            foreach (var pair in itemsToAdd)
            {
                o.Add(pair.Key, pair.Value);
            }

            return o;
        }

        protected internal override void SetToken(JToken token)
        {
            this.Settings = (JObject)token ?? new JObject();

            foreach (var component in this)
            {
                component.SetToken(this.Settings[component.Id]);
            }
        }

        internal static void SetLanguage(Language language)
        {
            if (Language == language)
            {
                return;
            }

            Language = language;

            LanguageChanged?.SafeInvoke(nameof(LanguageChanged));
        }

        private static bool released;

        private static bool toggle;

        internal static void SetBehavior(WindowMessageWParam button, bool toggleBehaviour)
        {
            SetVisibility(false);

            ActivationButton = button;

            toggle = toggleBehaviour;

            released = false;
        }

        private static void SetVisibility(bool open)
        {
            if (IsOpen == open)
            {
                return;
            }

            IsOpen = open;

            VisibilityStateChanged.SafeInvoke(nameof(VisibilityStateChanged));
        }

        private static void WndProc(GameWndProcEventArgs args)
        {
            if (GameConsole.IsOpen || ItemShop.IsOpen)
            {
                SetVisibility(false);
                return;
            }

            if (args.WParam != ActivationButton)
            {
                return;
            }

            var m = args.Message;

            if (IsOpen ? m == WindowMessage.KEYUP && (!toggle || !(released ^= true)) : m == WindowMessage.KEYDOWN)
            {
                SetVisibility(!IsOpen);
            }
        }

        #endregion
    }
}