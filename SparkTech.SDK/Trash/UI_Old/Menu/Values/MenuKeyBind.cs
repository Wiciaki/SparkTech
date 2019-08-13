namespace SparkTech.SDK.UI.Menu.Values
{
    using System.Drawing;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Util;

    public class MenuKeyBind : MenuValue, IMenuValue<bool>, IMenuValue<WindowsMessagesWParam>
    {
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

                this.OnPropertyChanged("Toggle", false);
                this.Value = false;
            }
        }

        private bool active, released, selecting;

        private string text;

        public MenuKeyBind(string id, WindowsMessagesWParam key) : base(id, (int)key)
        {
            GameEvents.OnWndProc += this.GameOnWndProc;
        }

        private void GameOnWndProc(WndProcEventArgs args)
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

        protected internal override void OnWndProc(Point point, Size size, WndProcEventArgs args)
        {
            if (!args.Message.IsLeftClick())
            {
                return;
            }

            point.X += size.Width - this.textSize.Width;

            if (!Mouse.IsInside(point, this.textSize))
            {
                return;
            }

            this.SetSelecting(!this.selecting);

            this.UpdateSize();
        }

        protected override Size GetSize()
        {
            var size = base.GetSize();

            this.text = this.selecting ? "Press the new key..." : this.key.ToString();

            var t = Theme.MeasureSize(this.text);

            if (t.Width < size.Height)
            {
                t.Width = size.Height;
            }

            size.Width += t.Width;

            this.textSize = t;

            return size;
        }

        private Size textSize;

        protected internal override void OnEndScene(Point point, Size size)
        {
            size.Width -= this.textSize.Width;

            base.OnEndScene(point, size);

            point.X += size.Width;

            Theme.Draw(new DrawData(point, this.textSize) { Text = this.text, ForceTextCentered = true, BackgroundColor = this.active ? Color.OrangeRed : Color.DarkRed});
        }

        private WindowsMessagesWParam key;

        WindowsMessagesWParam IMenuValue<WindowsMessagesWParam>.Value
        {
            get => this.key;
            set
            {
                if (this.SetKey(value))
                {
                    this.OnPropertyChanged("Key");
                }
            }
        }

        private bool toggle, wasActive;

        private void SetSelecting(bool b)
        {
            if (this.selecting == b)
            {
                return;
            }

            this.selecting = b;

            this.wasActive = this.active;

            this.Value = this.released = false;
        }

        private bool SetKey(WindowsMessagesWParam param)
        {
            if (this.key == param)
            {
                return false;
            }

            this.SetSelecting(false);

            if (param == this.key && this.wasActive)
            {
                this.Value = true;
            }

            this.key = param;

            this.UpdateSize();

            return true;
        }

        public bool Value
        {
            get => this.active;
            set
            {
                if (this.active == value)
                {
                    return;
                }

                this.active = value;

                this.OnPropertyChanged("IsActive", false);
            }
        }

        protected override JToken Token
        {
            get => (int)this.key;
            set => this.SetKey((WindowsMessagesWParam)value.Value<int>());
        }
    }
}
