namespace SparkTech.SDK.GUI.Menu.Items
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Misc;

    public class MenuKeyBind : MenuValue, IMenuValue<bool>, IMenuValue<WindowsMessagesWParam>
    {
        public MenuKeyBind(string id, WindowsMessagesWParam key) : base(id, key.ToString())
        {
            GameEvents.OnWndProc += this.WndProc;
        }

        private bool value, toggle, selecting, released;

        private string text;

        private WindowsMessagesWParam key;

        private Size2 size;

        public bool Value
        {
            get => this.value;
            set => this.value ^= this.value != value && this.UpdateValue(value);
        }

        WindowsMessagesWParam IMenuValue<WindowsMessagesWParam>.Value
        {
            get => this.key;
            set
            {
                if (this.key != value && this.UpdateValue(value))
                {
                    this.key = value;

                    this.UpdateSize();
                }
            }
        }

        public bool Toggle
        {
            get => this.toggle;
            set
            {
                if (this.toggle == value)
                {
                    return;
                }

                this.toggle = value;

                this.released = false;
                this.Value = false;
            }
        }

        private void WndProc(WndProcEventArgs args)
        {
            var m = args.Message;

            if (this.selecting)
            {
                if (m == WindowsMessages.KEYUP)
                {
                    this.SetValue(args.WParam);
                }

                return;
            }

            if (args.WParam == this.key)
            {
                this.Value = m == WindowsMessages.KEYDOWN || m == WindowsMessages.KEYUP && (!this.toggle || (this.released ^= true));
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

            this.Value = false;
            this.released = false;

            this.UpdateSize();
        }

        protected override Size2 GetSize()
        {
            this.text = this.selecting ? "Select..." : this.key.ToString();

            var s = base.GetSize();

            var t = Theme.MeasureText(this.text);
            t.Height = s.Height;
            this.size = t;

            s.Width += t.Width;

            return s;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;
            base.OnEndScene(point, width);
            point.X += width;

            var color = this.selecting ? Theme.BackgroundColor : this.value ? Color.OrangeRed : Color.DarkRed;

            Theme.DrawTextBox(point, this.size, this.text, color);
        }

        protected override JToken Token
        {
            get => this.key.ToString();
            set => this.SetValue(EnumCache<WindowsMessagesWParam>.Parse(value.Value<string>()));
        }
    }
}