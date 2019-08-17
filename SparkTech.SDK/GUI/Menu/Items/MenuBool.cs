namespace SparkTech.SDK.GUI.Menu.Items
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.Game;

    public class MenuBool : MenuValue, IMenuValue<bool>
    {
        public MenuBool(string id, bool defaultValue) : base(id, defaultValue)
        {

        }

        private bool value;

        private Size2 size;

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

            this.size = new Size2(28, s.Height);

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