namespace SparkTech.SDK.GUI.Menu
{
    using System;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    public class MenuBool : MenuValue, IMenuValue<bool>
    {
        public MenuBool(string id, bool defaultValue) : base(id, defaultValue)
        {

        }

        private Size2 size;

        private bool value;

        public bool Value
        {
            get => this.value;
            set => this.value ^= this.value != value && this.UpdateValue(value);
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawBox(point, this.size, this.Value ? Color.Green : Color.Red);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            point.X += width - this.size.Width;
            this.Value ^= Menu.IsCursorInside(point, this.size) && Menu.IsLeftClick(args.Message);
        }

        protected override Size2 GetSize()
        {
            var s = base.GetSize();
            var extraWidth = Math.Min(56, s.Height);
            this.size = new Size2(extraWidth, s.Height);
            s.Width += this.size.Width;

            return s;
        }

        protected override JToken Token
        {
            get => this.value;
            set => this.value = value.Value<bool>();
        }
    }
}