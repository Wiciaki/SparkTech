namespace Surgical.SDK.GUI.Menu
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using Surgical.SDK.API;
    using Surgical.SDK.EventData;

    public class MenuKey : MenuValue, IMenuValue<Key>
    {
        public MenuKey(string id, Key defaultValue) : base(id, defaultValue.ToString())
        {
            UserInput.OnWndProc += this.KeyWndProc;
        }

        private bool selecting;

        private string text;

        private Size2 size;

        private Key value;

        public Key Value
        {
            get => this.value;
            set
            {
                if (this.value == Key.None)
                {
                    this.value = value;
                    return;
                }

                if (this.value == value || !this.UpdateValue(value))
                {
                    return;
                }

                this.value = value;
                this.selecting = false;

                this.UpdateSize();
            }
        }

        protected virtual Color ButtonColor => this.selecting ? Color.OrangeRed : Color.DarkOrange;

        protected virtual void KeyWndProc(WndProcEventArgs args)
        {
            if (this.selecting && args.Message == WindowsMessages.KEYUP)
            {
                this.Value = args.Key;
            }
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            if (!Menu.IsLeftClick(args.Message))
            {
                return;
            }

            point.X += width - this.size.Width;

            if (!Menu.IsCursorInside(point, this.size))
            {
                return;
            }

            this.selecting ^= true;

            this.UpdateSize();
        }

        protected override Size2 GetSize()
        {
            this.text = this.selecting ? SdkSetup.GetTranslatedString("keySelector") : this.value.ToString();

            var s = base.GetSize();

            var t = Theme.MeasureText(this.text);
            t.Height = s.Height;

            if (Theme.MinItemHeight > t.Width)
            {
                t.Width = Theme.MinItemHeight;
            }

            this.size = t;

            s.Width += t.Width;
            return s;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawTextBox(point, this.size, this.ButtonColor, this.text, true);
            Theme.DrawBorders(point, this.size);
        }

        protected override JToken Token
        {
            get => this.Value.ToString();
            set => this.Value = EnumCache<Key>.Parse(value.Value<string>());
        }
    }
}