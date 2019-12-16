namespace Surgical.SDK.GUI.Menu
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

    using Surgical.SDK.API;
    using Surgical.SDK.EventData;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Rendering;

    public class Menu : MenuText, IEnumerable<MenuItem>, IExpandable
    {
        private readonly List<MenuItem> items = new List<MenuItem>();

        private readonly Translations translations = new Translations();

        private JObject settings;

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
            Build(menu, translations, true);
        }

        internal static void Build(Menu menu, JObject translations, bool createSaveHandler)
        {
            if (Roots.Exists(root => root.Id == menu.Id))
            {
                throw new InvalidOperationException("Menu with id \"" + menu.Id + "\" already exists as root!");
            }

            Roots.Add(menu);

            if (translations != null)
            {
                menu.translations.Set(translations);
            }
            else
            {
                menu.UpdateSizes();
            }

            if (createSaveHandler)
            {
                menu.CreateSaveHandler(Folder.Menu);
            }
            else
            {
                menu.SetToken(null);
            }

            menu.UpdateTranslations();
        }

        internal void CreateSaveHandler(Folder folder)
        {
            SaveHandlers.Add(new SaveHandler(this, folder));
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

            EndSceneGroup(this.items, point);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
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
            if (!this.IsSaving)
            {
                return false;
            }

            var b = false;

            foreach (var _ in this.Where(item => item.ConsumeSaveToken()))
            {
                b = true;
            }

            return b;
        }

        protected internal override JToken GetToken()
        {
            if (!this.IsSaving)
            {
                return null;
            }

            // broscience but works rly fast
            var b = false;
            var addable = from item in this let token = item.GetToken() where token != null && (b = true) select (item.Id, Token: token);

            if (!b)
            {
                return null;
            }

            var o = new JObject();

            foreach (var (id, token) in addable)
            {
                o.Add(id, token);
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

        private void UpdateTranslations()
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

        private static readonly List<Menu> Roots = new List<Menu>();

        private static readonly List<SaveHandler> SaveHandlers = new List<SaveHandler>();

        public static Keys ActivationKeys { get; private set; }

        public static bool IsOpen { get; private set; }

        public static Language Language { get; private set; }

        public static string LanguageTag { get; private set; }

        public static event Action<EventArgs> OnVisibilityChanged, OnLanguageChanged;

        private static bool released;

        private static bool toggleBehavior;

        static Menu()
        {
            LanguageTag = EnumCache<Language>.Description(default);

            //Game.OnStart += delegate
            {
                Render.OnEndScene += OnEndScene;
                WndProc.OnWndProc += OnWndProc;
            };
        }

        private static void OnEndScene()
        {
            if (!IsOpen)
            {
                return;
            }

            if (Platform.HasWndProc)
            {
                cursor = WndProc.CursorPosition;
            }

            EndSceneGroup(Roots, position);
        }

        private static void OnWndProc(WndProcEventArgs args)
        {
            if (Platform.HasAPI)
            {
                if (Game.IsChatOpen() || Game.IsShopOpen())
                {
                    SetOpen(false);
                    return;
                }
            }

            if (args.Keys == ActivationKeys)
            {
                var m = args.Message;

                if (IsOpen ? m == WindowsMessages.KEYUP && (!toggleBehavior || !(released ^= true)) : m == WindowsMessages.KEYDOWN)
                {
                    SetOpen(!IsOpen);
                    return;
                }
            }

            Log.Info(args);
            Log.Info($"CursonPos: {cursor}");

            if (IsOpen)
            {
                WndProcGroup(Roots, position, args);
            }
        }

        #region Group operations

        private static void WndProcGroup<T>(List<T> roots, Point point, WndProcEventArgs args) where T : MenuItem
        {
            if (roots.Count == 0)
            {
                return;
            }

            var width = roots.Max(item => item.Size.Width);

            roots.ForEach(item =>
            {
                item.OnWndProc(point, width, args);

                if (!item.IsVisible)
                {
                    return;
                }

                if (IsLeftClick(args.Message) && item is IExpandable && IsCursorInside(point, new Size2(width, item.Size.Height)))
                {
                    foreach (var expandable in roots.OfType<IExpandable>())
                    {
                        expandable.IsExpanded = expandable == item && !expandable.IsExpanded;
                    }
                }

                point.Y += item.Size.Height;
            });
        }

        private static void EndSceneGroup<T>(List<T> roots, Point point) where T : MenuItem
        {
            roots = roots.FindAll(item => item.IsVisible);

            if (roots.Count == 0)
            {
                return;
            }

            var p = point;
            var width = roots.Max(item => item.Size.Width);

            roots.ForEach(item =>
            {
                item.OnEndScene(point, width);

                point.Y += item.Size.Height;
            });

            var sizes = new Size2[roots.Count];

            for (var i = 0; i < roots.Count; i++)
            {
                sizes[i] = new Size2(width, roots[i].Size.Height);
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
                SaveHandlers.ForEach(r => r.Save());
            }

            OnVisibilityChanged.SafeInvoke(EventArgs.Empty);
        }

        private static Point position;

        internal static void SetPosition(int x, int y)
        {
            position = new Point(x, y);
        }

        internal static void SetTriggers(Keys keys, bool toggle)
        {
            if (toggle == toggleBehavior && ActivationKeys == keys)
            {
                return;
            }

            ActivationKeys = keys;
            toggleBehavior = toggle;
            released = false;

            SetOpen(false);
        }

        internal static void SetLanguage(int languageIndex)
        {
            SetLanguage(EnumCache<Language>.Values[languageIndex]);
        }

        private static void SetLanguage(Language language)
        {
            var tag = EnumCache<Language>.Description(language);

            if (LanguageTag == tag)
            {
                return;
            }

            Language = language;
            LanguageTag = tag;

            Roots.ForEach(root => root.UpdateTranslations());

            OnLanguageChanged.SafeInvoke(EventArgs.Empty);
        }

        internal static void SetArrows(bool value)
        {
            ArrowsEnabled = value;
        }

        #endregion

        #region Static Helper Methods

        private static Vector2 cursor;

        internal static bool IsCursorInside(Point point, Size2 size)
        {
            var rect = point.ToRectangle(size);

            // hello this is bugged af

            return cursor.X >= rect.Left && cursor.X <= rect.Right && cursor.Y <= rect.Bottom && cursor.Y >= rect.Top;
        }

        internal static bool IsLeftClick(WindowsMessages message)
        {
            return message == WindowsMessages.LBUTTONDOWN || message == WindowsMessages.LBUTTONDBLCLK;
        }

        public static void UpdateAllSizes()
        {
            Roots.ForEach(root => root.UpdateSizes());
        }

        private void UpdateSizes()
        {
            foreach (var item in this.GetDescensants().Prepend(this))
            {
                item.UpdateSize();
            }
        }

        internal static bool ArrowsEnabled { get; private set; }

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

        #region SaveHandler

        private class SaveHandler : IDisposable
        {
            private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

            private readonly Menu menu;

            private readonly string targetPath;

            private JToken lastSaved;

            public SaveHandler(Menu menu, Folder folder)
            {
                this.menu = menu;
                this.targetPath = folder.GetFile(menu.Id + ".json");

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
                        Log.Error($"Failed to load JSON file for \"{this.menu.Id}\"");
                        Log.Error(ex);
                    }
                }

                this.menu.SetToken(o);
            }

            public async void Save()
            {
                await this.semaphore.WaitAsync();

                if (this.menu.ConsumeSaveToken())
                {
                    var token = this.menu.GetToken();

                    if (token == null)
                    {
                        Log.Info("All values are default, nothing to save...");
                        this.lastSaved = null;
                        File.Delete(this.targetPath);
                    }
                    else if (JToken.DeepEquals(token, this.lastSaved))
                    {
                        Log.Info("Nothing needs saving, aborting...");
                    }
                    else
                    {
                        this.lastSaved = token;

                        using (var fs = new FileStream(this.targetPath, FileMode.Create, FileAccess.Write))
                        {
                            using var sw = new StreamWriter(fs);
                            using var writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented };
                            
                            await token.WriteToAsync(writer);
                        }

                        Log.Info($"Saved the updated values for \"{this.menu.Id}\"!");
                    }
                }

                this.semaphore.Release();
            }

            public void Dispose()
            {
                this.semaphore.Dispose();
            }
        }

        #endregion
    }
}