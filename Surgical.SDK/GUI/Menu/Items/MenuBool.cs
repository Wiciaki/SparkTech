namespace Surgical.SDK.GUI.Menu
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using Surgical.SDK.EventData;

    public class MenuBool : MenuValue, IMenuValue<bool>
    {
        private Size2 size;

        private bool value;

        public MenuBool(string id, bool defaultValue) : base(id, defaultValue)
        {

        }

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
            Theme.DrawBorders(point, this.size);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            point.X += width - this.size.Width;

            this.Value ^= Menu.IsLeftClick(args.Message) && Menu.IsCursorInside(point, this.size);
        }

        protected override Size2 GetSize()
        {
            return AddButton(base.GetSize(), out this.size);
        }

        protected override JToken Token
        {
            get => this.value;
            set => this.value = value.Value<bool>();
        }
    }
}