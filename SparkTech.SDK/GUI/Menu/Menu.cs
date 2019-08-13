namespace SparkTech.SDK.GUI.Menu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Misc;
    using SparkTech.SDK.Rendering;

    public class Menu : MenuText, IEnumerable<MenuItem>
    {
        private readonly List<MenuItem> items = new List<MenuItem>();

        protected JObject Settings;

        private Size arrowBoxSize;

        private const string ExpandText = ">>";

        public Menu(string id) : base(id)
        {

        }

        protected override void UpdateTextBasedSize()
        {
            var width = Math.Max(this.TextSize.Height, Theme.MeasureText(ExpandText).Width);

            this.arrowBoxSize = new Size(width, this.TextSize.Height);
        }

        protected override Size GetSize()
        {
            var b = base.GetSize();

            b.Width += this.arrowBoxSize.Width;

            return b;
        }

        public bool IsExpanded { get; private set; }

        public IEnumerable<TMenuItem> GetItems<TMenuItem>() where TMenuItem : MenuItem => this.OfType<TMenuItem>();

        public MenuItem this[string id] => this.items.Find(item => item.Id == id);

        public Menu GetMenu(string id) => this.GetItems<Menu>().FirstOrDefault(item => item.Id == id);

        public IEnumerable<MenuItem> GetDescensants() => this.Concat(this.GetItems<Menu>().SelectMany(menu => menu.GetDescensants()));

        protected internal override void OnEndScene(Point point, int groupWidth)
        {
            base.OnEndScene(point, groupWidth);

            point.X += this.TextSize.Width;

            Theme.DrawTextBox(ExpandText, point, this.arrowBoxSize);

            if (!this.IsExpanded)
            {
                return;
            }

            point.X += Theme.ItemGroupDistance;

            DrawGroup(this.items, point);
        }

        protected internal override void OnWndProc(Point point, int groupWidth, WndProcEventArgs args)
        {
            point.X += groupWidth - this.arrowBoxSize.Width;

            switch (args.Message)
            {
                case WindowsMessages.LBUTTONDOWN:
                case WindowsMessages.LBUTTONDBLCLK:
                    break;
                default:
                    return;
            }

            if (IsCursorInside(point, this.arrowBoxSize))
            {
                this.IsExpanded ^= true;
            }
        }

        public void Add(MenuItem item)
        {
            if (this.items.Exists(i => i.Id == item.Id))
            {
                throw new InvalidOperationException($"Id \"{item.Id}\" already exists within this menu instance!");
            }

            this.items.Add(item);

            if (this.Settings == null)
            {
                return;
            }

            item.SetToken(this.Settings[item.Id]);
        }

        protected internal override bool ShouldSave()
        {
            return this.Count(item => item.ShouldSave()) > 0;
        }

        protected internal override JToken GetToken()
        {
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
            this.Settings = (JObject)token ?? new JObject();

            // todo translation stuffs using JObject.Merge

            foreach (var item in this.items)
            {
                item.SetToken(this.Settings[item.Id]);
            }
        }

        private static readonly List<Menu> Roots = new List<Menu>();

        public static WindowsMessagesWParam ActivationButton { get; private set; }

        public static bool IsOpen { get; private set; }

        public static event Action VisibilityChanged;

        private static bool released;

        private static bool toggle;

        static Menu()
        {
            Render.OnEndScene += OnEndScene;
            GameEvents.OnWndProc += OnWndProc;
        }

        public static IReadOnlyList<Menu> GetRootMenus()
        {
            return Roots.AsReadOnly();
        }

        public static void AddToRoot(Menu menu)
        {
            Roots.Add(menu);
        }

        private static void OnEndScene()
        {
            if (!IsOpen)
            {
                return;
            }

            DrawGroup(Roots, GetRootPoint());
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

                var visibleRoots = Roots.FindAll(item => item.IsVisible);
                var width = visibleRoots.Max(item => item.Size.Width);

                visibleRoots.ForEach(item => item.OnWndProc(point, width, args));
            }
        }

        private static void SetMenuVisibility(bool open)
        {
            if (IsOpen == open)
            {
                return;
            }

            IsOpen = open;

            VisibilityChanged.SafeInvoke();
        }

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

        private static void DrawGroup<T>(List<T> items, Point point) where T : MenuItem
        {
            items = items.FindAll(item => item.IsVisible);

            var width = items.Max(item => item.Size.Width);

            items.ForEach(item => item.OnEndScene(point, width));
        }

        private static Point GetRootPoint()
        {
            return new Point(25, 25);
        }

        IEnumerator<MenuItem> IEnumerable<MenuItem>.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<MenuItem>)this).GetEnumerator();
        }

        private static Point cursor;

        internal static bool IsCursorInside(Point point, Size size)
        {
            var rectangle = point.ToRectangle(size);

            return cursor.X >= rectangle.Left && cursor.X <= rectangle.Right && cursor.Y >= rectangle.Bottom && cursor.Y <= rectangle.Top;
        }
    }
}