namespace SparkTech.SDK.GUI.Menu
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Platform;

    public class MenuKey : MenuValue, IMenuValue<WindowsMessagesWParam>
    {
        public MenuKey(string id, WindowsMessagesWParam defaultValue) : base(id, defaultValue.ToString())
        {
            GameEvents.OnWndProc += this.WndProc;
        }

        private bool selecting;

        private string text;

        private Size2 size;

        private WindowsMessagesWParam value;

        public WindowsMessagesWParam Value
        {
            get => this.value;
            set
            {
                if (this.value == value || !this.UpdateValue(value))
                {
                    return;
                }

                this.value = value;
                this.selecting = false;

                this.UpdateSize();
            }
        }

        protected virtual Color ButtonColor => this.selecting ? Color.OrangeRed : Theme.BackgroundColor;

        protected virtual void WndProc(WndProcEventArgs args)
        {
            if (this.selecting && args.Message == WindowsMessages.KEYUP)
            {
                this.Value = args.WParam;
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
            this.size = t;

            s.Width += t.Width;

            return s;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawTextBox(point, this.size, this.text, true, this.ButtonColor);
        }

        protected override JToken Token
        {
            get => this.Value.ToString();
            set => this.Value = EnumCache<WindowsMessagesWParam>.Parse(value.Value<string>());
        }
    }
}