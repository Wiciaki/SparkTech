namespace SparkTech.SDK.GUI.Menu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Misc;
    using SparkTech.SDK.Rendering;
    using SparkTech.SDK.Security;

    public class Menu : MenuText, IEnumerable<MenuItem>
    {
        private readonly List<MenuItem> items = new List<MenuItem>();

        private JObject settings;

        private Size size;

        private const string ExpandText = ">>";

        public Menu(string id) : base(id)
        {

        }

        public bool IsExpanded { get; private set; }

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

        public static void Root(Menu menu, JObject translations = null)
        {
            void Set() => menu.SetTranslations(translations);

            Set();
            LanguageChanged += Set;

            RootEntries.Add(new RootEntry(menu));
        }

        #endregion

        #region Overrides

        protected override Size GetSize()
        {
            var width = Math.Max(28, Theme.MeasureText(ExpandText).Width);

            var b = base.GetSize();
            b.Width += width;

            this.size = new Size(width, b.Height);

            return b;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawTextBox(point, this.size, ExpandText);

            if (!this.IsExpanded)
            {
                return;
            }

            point.X += this.size.Width + Theme.ItemGroupDistance;

            DrawGroup(this.items, point);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            point.X += width - this.size.Width;

            this.IsExpanded ^= IsLeftClick(args.Message) && IsCursorInside(point, this.size);
        }

        public void Add(MenuItem item)
        {
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

            foreach (var (key, value) in addable)
            {
                o.Add(key, value);
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

        protected internal override void SetTranslations(JObject json)
        {
            if (json == null)
            {
                return;
            }

            base.SetTranslations(LanguageAccess(json));

            foreach (var item in this)
            {
                var o = (JObject)json[item.Id];

                if (o == null)
                {
                    continue;
                }

                if (!(item is Menu))
                {
                    o = LanguageAccess(o);
                }

                if (o != null)
                {
                    item.SetTranslations(o);
                }
            }

            static JObject LanguageAccess(JObject o) => (JObject)(o[LanguageTag] ?? o["en"]);
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

        public static WindowsMessagesWParam ActivationButton { get; private set; } = WindowsMessagesWParam.Shift;

        public static bool IsOpen { get; private set; }

        public static Language Language { get; private set; }

        public static string LanguageTag { get; private set; }

        public static event Action VisibilityChanged, LanguageChanged;

        private static bool released;

        private static bool toggle;

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

            cursor = GameInterface.CursorPosition().ToPoint();

            DrawGroup(RootEntries.ConvertAll(e => e.Menu), GetRootPoint());
        }

        private static void OnWndProc(WndProcEventArgs args)
        {
            if (GameInterface.IsChatOpen() || GameInterface.IsShopOpen())
            {
                SetMenuVisibility(false);
                return;
            }

            if (args.WParam == ActivationButton)
            {
                var m = args.Message;

                if (IsOpen ? m == WindowsMessages.KEYUP && (!toggle || !(released ^= true)) : m == WindowsMessages.KEYDOWN)
                {
                    SetMenuVisibility(!IsOpen);
                }
            }

            if (IsOpen)
            {
                var point = GetRootPoint();

                var visibleRoots = RootEntries.ConvertAll(e => e.Menu).FindAll(item => item.IsVisible);
                var width = visibleRoots.Max(item => item.Size.Width);

                visibleRoots.ForEach(item =>
                {
                    item.OnWndProc(point, width, args);

                    point.Y += item.Size.Height;
                });
            }
        }

        #region Menu Settings

        internal static void SetMenuTriggers(WindowsMessagesWParam button, bool toggleBehaviour)
        {
            if (toggleBehaviour == toggle && ActivationButton == button)
            {
                return;
            }

            ActivationButton = button;

            toggle = toggleBehaviour;

            released = false;

            SetMenuVisibility(false);
        }

        internal static void SetLanguage(Language language)
        {
            if (Language == language)
            {
                return;
            }

            Language = language;

            LanguageTag = EnumCache<Language>.Description(language);

            LanguageChanged.SafeInvoke();
        }

        #endregion

        #region Menu Position

        private static Point GetRootPoint()
        {
            return new Point(25, 25);
        }

        #endregion

        #region Static Helper Methods

        private static Point cursor;

        internal static bool IsCursorInside(Point point, Size size)
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

        private static void SetMenuVisibility(bool open)
        {
            if (IsOpen == open)
            {
                return;
            }

            IsOpen = open;

            RootEntries.ForEach(r => r.Save());

            VisibilityChanged.SafeInvoke();
        }

        private static void DrawGroup<T>(List<T> items, Point point) where T : MenuItem
        {
            items = items.FindAll(item => item.IsVisible);

            var width = items.Max(item => item.Size.Width);

            items.ForEach(item =>
            {
                item.OnEndScene(point, width);

                point.Y += item.Size.Height;
            });
        }

        #endregion

        #region RootEntry

        private class RootEntry
        {
            public readonly Menu Menu;

            private readonly string targetPath;

            private JToken lastSaved;

            public RootEntry(Menu menu)
            {
                this.Menu = menu;

                this.targetPath = Folder.MenuFolder.GetFile(menu.Id + ".json");

                var o = new JObject();

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

                this.Menu.settings = o;
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

                await Folder.SaveMenuTokenAsync(this.targetPath, token);

                Log.Info($"Saving completed for \"{this.Menu.Id}\"!");
            }
        }

        #endregion
    }
}