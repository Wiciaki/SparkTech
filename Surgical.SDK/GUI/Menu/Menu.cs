﻿namespace Surgical.SDK.GUI.Menu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using Surgical.SDK.EventData;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Rendering;
    using Surgical.SDK.Security;

    public class Menu : MenuText, IEnumerable<MenuItem>, IExpandable
    {
        private readonly List<MenuItem> items = new List<MenuItem>();

        private JObject settings;

        private Translations translations;

        private Size2 size;

        internal const string ArrowText = ">>";

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

        public TMenuItem Get<TMenuItem>(string id) where TMenuItem : MenuItem
        {
            return (TMenuItem)this[id];
        }

        public Menu GetMenu(string id)
        {
            return this.Get<Menu>(id);
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

            if (translations == null)
            {
                menu.UpdateSizes();
            }

            menu.translations = new Translations(translations);

            RootEntries.Add(new RootEntry(menu));
        }

        public static void Radio(MenuBool[] items)
        {
            var block = false;

            foreach (var item in items)
            {
                item.BeforeValueChange += args =>
                {
                    if (block)
                    {
                        return;
                    }

                    block = true;

                    foreach (var i in items)
                    {
                        i.Value = i.Id == item.Id;
                    }

                    block = false;
                };
            }
        }

        #endregion

        #region Overrides

        protected override Size2 GetSize()
        {
            return AddButton(base.GetSize(), out this.size, ArrowText);
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawTextBox(point, this.size, ArrowText, true, this.BackgroundColor);

            if (!this.IsExpanded)
            {
                return;
            }

            point.X += this.size.Width;

            if (ArrowsEnabled)
            {
                AddArrow(point);
                point.X += ArrowWidth;
            }

            DrawGroup(this.items, point);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            if (!this.IsExpanded)
            {
                return;
            }

            point.X += width;

            if (ArrowsEnabled)
            {
                point.X += ArrowWidth;
            }

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

            if (this.settings == null)
            {
                return;
            }

            item.SetToken(this.settings[item.Id]);

            UpdateItemSize(item, this.translations);
        }

        public void Add(MenuItem item, JObject value)
        {
            if (value != null)
            {
                this.translations.Add(item.Id, value);
            }

            this.Add(item);
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

        private void UpdateLanguage()
        {
            this.SetTranslations(this.translations);
        }

        private static void UpdateItemSize(MenuItem item, Translations t)
        {
            var o = t.GetObject(item.Id);

            if (o != null)
            {
                item.SetTranslations(o);
            }
            else if (item is Menu menu)
            {
                menu.UpdateSizes();
            }
            else
            {
                item.UpdateSize();
            }
        }

        protected internal override void SetTranslations(Translations t)
        {
            this.translations = t;

            base.SetTranslations(t);

            foreach (var item in this)
            {
                UpdateItemSize(item, t);
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

        internal static bool ArrowsEnabled { get; private set; }

        private static readonly List<RootEntry> RootEntries = new List<RootEntry>();

        public static Key ActivationButton { get; private set; }

        public static bool IsOpen { get; private set; }

        public static Language Language { get; private set; }

        public static string LanguageTag { get; private set; }

        public static event Action<EventArgs> VisibilityChanged, LanguageChanged;

        private static bool released;

        private static bool toggleBehavior;

        static Menu()
        {
            LanguageTag = EnumCache<Language>.Description(default);

            Render.OnEndScene += OnEndScene;
            Game.OnWndProc += OnWndProc;
        }

        private static void OnEndScene()
        {
            if (!IsOpen)
            {
                return;
            }

            //cursor = Game.CursorPosition2D;

            DrawGroup(RootEntries.ConvertAll(e => e.Menu), position);
        }

        private static void OnWndProc(WndProcEventArgs args)
        {
            //if (GameInterface.IsChatOpen() || GameInterface.IsShopOpen())
            //{
            //    SetOpen(false);
            //    return;
            //}

            if (args.Key == ActivationButton)
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

                if (left && item is IExpandable)
                {
                    var size = item.Size;
                    size.Width = width;

                    if (IsCursorInside(point, size))
                    {
                        foreach (var i in items.OfType<IExpandable>())
                        {
                            i.IsExpanded = i == item && !i.IsExpanded;
                        }
                    }
                }

                point.Y += item.Size.Height;
            });
        }

        private static void DrawGroup<T>(List<T> items, Point point) where T : MenuItem
        {
            if (items.Count == 0)
            {
                return;
            }

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

            VisibilityChanged.SafeInvoke(EventArgs.Empty);
        }

        private static Point position;

        internal static void SetPosition(int x, int y)
        {
            position = new Point(x, y);
        }

        internal static void SetTriggers(Key button, bool toggle)
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

        internal static void SetLanguage(int languageIndex)
        {
            SetLanguage(EnumCache<Language>.Values[languageIndex]);

        }

        internal static void SetLanguage(Language language)
        {
            var tag = EnumCache<Language>.Description(language);

            if (LanguageTag == tag)
            {
                return;
            }

            Language = language;

            LanguageTag = tag;

            RootEntries.ForEach(entry => entry.Menu.UpdateLanguage());

            LanguageChanged.SafeInvoke(EventArgs.Empty);
        }

        internal static void SetArrows(bool b)
        {
            ArrowsEnabled = b;
        }

        #endregion

        #region Static Helper Methods

        private static Vector2 cursor;

        internal static bool IsCursorInside(Point point, Size2 size)
        {
            var rectangle = point.ToRectangle(size);

            return cursor.X >= rectangle.Left && cursor.X <= rectangle.Right && cursor.Y >= rectangle.Bottom && cursor.Y <= rectangle.Top;
        }

        internal static bool IsLeftClick(WindowsMessages message)
        {
            return message == WindowsMessages.LBUTTONDOWN || message == WindowsMessages.LBUTTONDBLCLK;
        }

        public static void UpdateAllSizes()
        {
            RootEntries.ForEach(entry => entry.Menu.UpdateSizes());
        }

        private void UpdateSizes()
        {
            foreach (var item in this.GetDescensants().Prepend(this))
            {
                item.UpdateSize();
            }
        }

        private static Size2 arrowSize;

        private const string ExtraArrowText = "➜";

        internal static int ArrowWidth => arrowSize.Width;

        internal static void AddArrow(Point point)
        {
            Theme.DrawTextBox(point, arrowSize, ExtraArrowText, true, Color.Transparent);
        }

        internal static void UpdateArrowSize()
        {
            arrowSize = new Size2(Theme.MeasureText(ExtraArrowText).Width, Theme.MinItemHeight);
        }

        #endregion

        #region RootEntry

        private class RootEntry
        {
            public readonly Menu Menu;

            private readonly string targetPath;

            private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

            private JToken lastSaved;

            public RootEntry(Menu menu)
            {
                this.Menu = menu;

                this.targetPath = Folder.Menu.GetFile(menu.Id + ".json");

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
                        Log.Error(ex);
                    }
                }

                this.Menu.SetToken(o);

                this.Menu.UpdateLanguage();
            }

            public async void Save()
            {
                await this.semaphore.WaitAsync();

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

                using (var fileStream = new FileStream(this.targetPath, FileMode.Create, FileAccess.Write))
                {
                    using var streamWriter = new StreamWriter(fileStream);
                    using var testWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented };

                    await token.WriteToAsync(testWriter);
                }

                Log.Info($"Saving completed for \"{this.Menu.Id}\"!");

                this.semaphore.Release();
            }
        }

        #endregion
    }
}