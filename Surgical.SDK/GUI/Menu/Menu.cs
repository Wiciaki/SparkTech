namespace Surgical.SDK.GUI.Menu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using Surgical.SDK.EventData;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Rendering;

    public class Menu : MenuText, IEnumerable<MenuItem>, IExpandable
    {
        private readonly List<MenuItem> items = new List<MenuItem>();

        private readonly Translations translations = new Translations();

        private JObject? settings;

        private Size2 size;

        internal const string ArrowText = ">>";

        public Menu(string id) : base(id)
        {
        }

        public bool IsExpanded { get; set; }

        #region Accessors

        [Pure]
        public IEnumerable<TMenuItem> GetItems<TMenuItem>() where TMenuItem : MenuItem
        {
            return this.OfType<TMenuItem>();
        }

        [Pure]
        public MenuItem? this[string id] => this.items.Find(item => item.Id == id);

        [Pure]
        public TMenuItem? Get<TMenuItem>(string id) where TMenuItem : MenuItem
        {
            return (TMenuItem?)this[id];
        }

        [Pure]
        public Menu? GetMenu(string id)
        {
            return this.Get<Menu>(id);
        }

        [Pure]
        public IEnumerable<MenuItem> GetDescendants()
        {
            return this.Concat(this.GetItems<Menu>().SelectMany(menu => menu.GetDescendants()));
        }

        #endregion

        public static void Build(Menu menu, JObject? translations = null)
        {
            Build(menu, translations, true);
        }

        internal static void Build(Menu menu, JObject? translations, bool createSaveHandler)
        {
            if (Roots.Exists(root => root.Id == menu.Id))
            {
                throw new InvalidOperationException("Menu with id \"" + menu.Id + "\" already exists as root!");
            }

            Roots.Add(menu);

            if (translations != null)
            {
                menu.translations.Add(translations);
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

            Theme.DrawTextBox(point, this.size, this.BackgroundColor, ArrowText, true);

            if (!this.IsExpanded || this.items.Count == 0)
            {
                return;
            }

            point.X += this.size.Width;

            if (ArrowsEnabled)
            {
                DrawArrow(point);
                point.X += ArrowWidth;
            }

            EndSceneGroup(this.items, point);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            if (!this.IsExpanded || this.items.Count == 0)
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

        protected internal override bool InsideExpandableArea(Point point, int width)
        {
            return IsCursorInside(point, new Size2(width, this.Size.Height));
        }

        public void Add(MenuItem item)
        {
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

        public void Add(MenuItem item, JObject? value)
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

            foreach (var item in this)
            {
                if (item.ConsumeSaveToken())
                {
                    b = true;
                }
            }

            return b;
        }

        protected internal override JToken? GetToken()
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

        protected internal override void SetToken(JToken? token)
        {
            if (!this.IsSaving)
            {
                return;
            }

            this.settings = (JObject?)token ?? new JObject();

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

        public static Key ActivationKeys { get; private set; }

        public static bool IsOpen { get; private set; }

        public static Language Language { get; private set; }

        public static string LanguageTag { get; private set; }

        public static event Action<EventArgs>? OnVisibilityChanged, OnLanguageChanged;

        private static bool released;

        private static bool toggleBehavior;

        static Menu()
        {
            LanguageTag = EnumCache<Language>.Description(default)!;

            if (!Platform.HasUserInputAPI)
            {
                SetOpen(true);
            }

            if (Platform.HasCoreAPI)
            {
                Game.OnStart += Subscribe;
            }
            else
            {
                Subscribe(null);
            }

            static void Subscribe(EventArgs? _)
            {
                Render.OnEndScene += OnEndScene;
                Render.OnDraw += OnDraw;
                UserInput.OnWndProc += OnWndProc;
            }
        }

        private static void OnDraw()
        {
            if (Platform.HasUserInputAPI)
            {
                cursor = UserInput.CursorPosition;
            }
        }

        private static void OnEndScene()
        {
            if (IsOpen)
            {
                EndSceneGroup(Roots, position);
            }
        }

        private static void OnWndProc(WndProcEventArgs args)
        {
            if (Platform.HasCoreAPI)
            {
                if (Game.IsChatOpen() || Game.IsShopOpen())
                {
                    SetOpen(false);
                    return;
                }
            }

            if (args.Key == ActivationKeys)
            {
                var m = args.Message;

                if (IsOpen ? m == WindowsMessages.KEYUP && (!toggleBehavior || !(released ^= true)) : m == WindowsMessages.KEYDOWN || m == WindowsMessages.CHAR)
                {
                    SetOpen(!IsOpen);
                    return;
                }
            }

            if (IsOpen)
            {
                WndProcGroup(Roots, position, args);
            }
        }

        #region Group operations

        private static void WndProcGroup<T>(List<T> roots, Point point, WndProcEventArgs args) where T : MenuItem
        {
            roots = roots.FindAll(item => item.IsVisible);

            if (roots.Count == 0)
            {
                return;
            }

            var width = roots.Max(item => item.Size.Width);

            roots.ForEach(item =>
            {
                item.OnWndProc(point, width, args);

                if (IsLeftClick(args.Message) && item is IExpandable && item.InsideExpandableArea(point, width))
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

        private static void SetOpen(bool open)
        {
            if (IsOpen == open)
            {
                return;
            }

            IsOpen = open;

            if (!open)
            {
                SaveHandlers.ForEach(handler => handler.Save());
            }

            OnVisibilityChanged.SafeInvoke(EventArgs.Empty);
        }

        private static Point position;

        internal static void SetPosition(int x, int y)
        {
            position = new Point(x, y);
        }

        internal static void SetTriggers(Key keys, bool toggle)
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
            var tag = EnumCache<Language>.Description(language)!;

            if (LanguageTag == tag)
            {
                return;
            }

            Language = language;
            LanguageTag = tag;

            Roots.ForEach(menu => menu.UpdateTranslations());

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
            return point.X <= cursor.X && point.X + size.Width >= cursor.X && point.Y <= cursor.Y && point.Y + size.Height >= cursor.Y;
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
            foreach (var item in this.GetDescendants().Prepend(this))
            {
                item.UpdateSize();
            }
        }

        internal static bool ArrowsEnabled { get; private set; }

        private static Size2 arrowSize;

        private const string ExtraArrowText = "➜";

        internal static int ArrowWidth => arrowSize.Width;

        internal static int MinItemWidth { get; private set; }

        internal static int MinNotificationWidth { get; private set; }

        internal static void DrawArrow(Point point)
        {
            Theme.DrawTextBox(point, arrowSize, Color.Transparent, ExtraArrowText, true);
        }

        internal static void UpdateDecalSizes()
        {
            arrowSize = new Size2(Theme.MeasureText(ExtraArrowText).Width, Theme.MinItemHeight);

            const string MinWidthText = "This is enough";

            MinItemWidth = Theme.MeasureText(MinWidthText).Width;

            const string MinNotificationText = "This is enought for notifications";

            MinNotificationWidth = Theme.MeasureText(MinNotificationText).Width;
        }

        #endregion

        #region SaveHandler

        private class SaveHandler
        {
            private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

            private readonly Menu menu;

            private readonly string targetPath;

            private JToken? lastSaved;

            public SaveHandler(Menu menu, Folder folder)
            {
                this.menu = menu;
                this.targetPath = folder.GetFile(menu.Id + ".json");

                JObject? o = null;

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
                    string status;
                    var token = this.menu.GetToken();

                    if (token == null)
                    {
                        status = "All values are default, deleting save file";
                        
                        this.lastSaved = null;
                        File.Delete(this.targetPath);
                    }
                    else if (JToken.DeepEquals(token, this.lastSaved))
                    {
                        status = "Nothing needs saving";
                    }
                    else
                    {
                        status = "Saved the updated values";

                        this.lastSaved = token;

                        await using var fs = new FileStream(this.targetPath, FileMode.Create, FileAccess.Write);
                        await using var sw = new StreamWriter(fs);
                        using var writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented };

                        await token.WriteToAsync(writer);
                    }

                    Log.Info($"SaveHandler - \"{this.menu.Id}\" - {status}");
                }

                this.semaphore.Release();
            }
        }

        #endregion
    }
}