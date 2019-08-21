﻿namespace SparkTech.SDK.GUI.Menu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Rendering;
    using SparkTech.SDK.Security;

    public class Menu : MenuText, IEnumerable<MenuItem>, IExpandable
    {
        private readonly List<MenuItem> items = new List<MenuItem>();

        private JObject settings;

        private Size2 size, arrowSize;

        private const string ExpandText = ">>";

        public Menu(string id) : base(id)
        {

        }

        public bool IsExpanded { get; set; }

        #region Accessors

        public IEnumerable<TMenuItem> GetItems<TMenuItem>() where TMenuItem : MenuItem
        {
            return this.OfType<TMenuItem>();
        }

        public MenuItem this[string id] => this.items.Find(item => item.Id == id);

        public Menu GetMenu(string id)
        {
            return this.GetItems<Menu>().FirstOrDefault(item => item.Id == id);
        }

        public IEnumerable<MenuItem> GetDescensants()
        {
            return this.Concat(this.GetItems<Menu>().SelectMany(menu => menu.GetDescensants()));
        }

        public static void Build(Menu menu, JObject translations = null)
        {
            if (RootEntries.Exists(entry => entry.Menu.Id == menu.Id))
            {
                throw new InvalidOperationException("Menu with id \"" + menu.Id + "\" already exists as root!");
            }

            RootEntries.Add(new RootEntry(menu, new Translations(translations)));
        }

        #endregion

        #region Overrides

        protected override Size2 GetSize()
        {
            var s = AddButton(base.GetSize(), out this.size);

            this.arrowSize = new Size2(Theme.MeasureText("->").Width, s.Height);

            return s;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawTextBox(point, this.size, ExpandText, true);

            if (!this.IsExpanded)
            {
                return;
            }

            point.X += this.size.Width + Theme.ItemGroupDistance;

            Theme.DrawTextBox(point, this.arrowSize, "➜", true);

            point.X += this.arrowSize.Width;

            DrawGroup(this.items, point);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            if (!this.IsExpanded)
            {
                return;
            }

            point.X += width + Theme.ItemGroupDistance;

            WndProcGroup(this.items, point, args);
        }

        public void Add(MenuItem item)
        {
            if (item == null)
            {
                return;
            }

            if (this.items.Exists(i => i.Id == item.Id))
            {
                throw new InvalidOperationException($"Id \"{item.Id}\" already exists within this menu instance!");
            }

            this.items.Add(item);

            if (this.settings != null)
            {
                item.SetToken(this.settings[item.Id]);
            }
        }

        public bool IsSaving { get; set; } = true;

        protected internal override bool ConsumeSaveToken()
        {
            return this.IsSaving && this.Count(item => item.ConsumeSaveToken()) != 0;
        }

        protected internal override JToken GetToken()
        {
            if (!this.IsSaving)
            {
                return null;
            }

            var addable = new Dictionary<string, JToken>();

            foreach (var item in this)
            {
                var token = item.GetToken();

                if (token != null)
                {
                    addable.Add(item.Id, token);
                }
            }

            if (addable.Count == 0)
            {
                return null;
            }

            var o = new JObject();

            foreach (var pair in addable)
            {
                o.Add(pair.Key, pair.Value);
            }

            return o;
        }

        protected internal override void SetToken(JToken token)
        {
            if (!this.IsSaving)
            {
                return;
            }

            this.settings = (JObject)token ?? new JObject();

            foreach (var item in this)
            {
                item.SetToken(this.settings[item.Id]);
            }
        }

        protected internal override void SetTranslations(Translations t)
        {
            if (t == null)
            {
                return;
            }

            base.SetTranslations(t);

            foreach (var item in this)
            {
                var o = t.GetObject(item.Id);

                if (o != null)
                {
                    item.SetTranslations(o);
                }
            }
        }

        public IEnumerator<MenuItem> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        private static readonly List<RootEntry> RootEntries = new List<RootEntry>();

        public static WindowsMessagesWParam ActivationButton { get; private set; }

        public static bool IsOpen { get; private set; }

        public static Language Language { get; private set; }

        public static string LanguageTag { get; private set; } = EnumCache<Language>.Description(default);

        public static event Action VisibilityChanged, LanguageChanged;

        private static bool released;

        private static bool toggleBehavior;

        static Menu()
        {
            Render.OnEndScene += OnEndScene;
            GameEvents.OnWndProc += OnWndProc;
        }

        private static void OnEndScene()
        {
            if (!IsOpen)
            {
                return;
            }

            cursor = GameEvents.GetCursorPosition();

            DrawGroup(RootEntries.ConvertAll(e => e.Menu), position);
        }

        private static void OnWndProc(WndProcEventArgs args)
        {
            //if (GameInterface.IsChatOpen() || GameInterface.IsShopOpen())
            //{
            //    SetMenuVisibility(false);
            //    return;
            //}

            if (args.WParam == ActivationButton)
            {
                var m = args.Message;

                if (IsOpen ? m == WindowsMessages.KEYUP && (!toggleBehavior || !(released ^= true)) : m == WindowsMessages.KEYDOWN)
                {
                    SetOpen(!IsOpen);
                    return;
                }
            }

            if (IsOpen)
            {
                WndProcGroup(RootEntries.ConvertAll(e => e.Menu), position, args);
            }
        }

        #region Group operations

        private static void WndProcGroup<T>(List<T> items, Point point, WndProcEventArgs args) where T : MenuItem
        {
            items = items.FindAll(item => item.IsVisible);

            var width = items.Max(item => item.Size.Width);
            var left = IsLeftClick(args.Message);

            items.ForEach(item =>
            {
                item.OnWndProc(point, width, args);

                if (left && item is IExpandable && IsCursorInside(point, item.Size))
                {
                    foreach (var i in items.OfType<IExpandable>())
                    {
                        i.IsExpanded = i == item && !i.IsExpanded;
                    }
                }

                point.Y += item.Size.Height;
            });
        }

        private static void DrawGroup<T>(List<T> items, Point point) where T : MenuItem
        {
            var p = point;

            items = items.FindAll(item => item.IsVisible);

            var width = items.Max(item => item.Size.Width);

            items.ForEach(item =>
            {
                item.OnEndScene(point, width);

                point.Y += item.Size.Height;
            });

            var sizes = new Size2[items.Count];

            for (var i = 0; i < items.Count; i++)
            {
                sizes[i] = new Size2(width, items[i].Size.Height);
            }

            Theme.DrawBorders(p, sizes);
        }

        #endregion

        #region Menu Settings

        private static Point position;

        internal static void SetPosition(int x, int y)
        {
            position = new Point(x, y);
        }

        internal static void SetTriggers(WindowsMessagesWParam button, bool toggle)
        {
            if (toggle == toggleBehavior && ActivationButton == button)
            {
                return;
            }

            ActivationButton = button;

            toggleBehavior = toggle;

            released = false;

            SetOpen(false);
        }

        internal static void SetLanguage(Language language)
        {
            if (Language == language)
            {
                return;
            }

            Language = language;

            LanguageTag = EnumCache<Language>.Description(language);

            RootEntries.ForEach(entry => entry.UpdateLanguage());

            LanguageChanged.SafeInvoke();
        }

        #endregion

        #region Static Helper Methods

        private static Point cursor;

        internal static bool IsCursorInside(Point point, Size2 size)
        {
            var rectangle = point.ToRectangle(size);

            return cursor.X >= rectangle.Left && cursor.X <= rectangle.Right && cursor.Y >= rectangle.Bottom && cursor.Y <= rectangle.Top;
        }

        internal static bool IsLeftClick(WindowsMessages message)
        {
            switch (message)
            {
                case WindowsMessages.LBUTTONDOWN:
                case WindowsMessages.LBUTTONDBLCLK:
                    return true;
                default:
                    return false;
            }
        }

        public static void UpdateAllSizes()
        {
            var roots = RootEntries.ConvertAll(entry => entry.Menu);

            foreach (var item in roots.Concat(roots.SelectMany(menu => menu.GetDescensants())))
            {
                item.UpdateSize();
            }
        }

        internal static void SetOpen(bool open)
        {
            if (IsOpen == open)
            {
                return;
            }

            IsOpen = open;

            if (!open)
            {
                RootEntries.ForEach(r => r.Save());
            }

            VisibilityChanged.SafeInvoke();
        }

        #endregion

        #region RootEntry

        private class RootEntry
        {
            public readonly Menu Menu;

            private readonly Translations translations;

            private readonly string targetPath;

            private JToken lastSaved;

            public RootEntry(Menu menu, Translations translations)
            {
                this.Menu = menu;

                this.translations = translations;

                this.targetPath = Folder.MenuFolder.GetFile(menu.Id + ".json");

                JObject o = null;

                if (File.Exists(this.targetPath))
                {
                    using var sr = new StreamReader(this.targetPath);
                    using var reader = new JsonTextReader(sr);

                    try
                    {
                        o = (JObject)JToken.ReadFrom(reader);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to load JSON file for \"{this.Menu.Id}\"");
                        ex.Log();
                    }
                }

                this.Menu.SetToken(o);

                this.UpdateLanguage();
            }

            public void UpdateLanguage()
            {
                this.Menu.SetTranslations(this.translations);
            }

            public async void Save()
            {
                if (!this.Menu.ConsumeSaveToken())
                {
                    return;
                }

                Log.Info($"Saving the updated values for \"{this.Menu.Id}\"...");

                var token = this.Menu.GetToken();

                if (token == null)
                {
                    Log.Info("All values are default, nothing to save...");
                    this.lastSaved = null;
                    File.Delete(this.targetPath);
                    return;
                }

                if (JToken.DeepEquals(this.lastSaved, token))
                {
                    Log.Info("Nothing needs saving, aborting...");
                    return;
                }

                this.lastSaved = token;

                await Folder.SaveTokenAsync(this.targetPath, token);

                Log.Info($"Saving completed for \"{this.Menu.Id}\"!");
            }
        }

        #endregion
    }
}