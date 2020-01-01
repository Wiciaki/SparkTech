namespace Surgical.SDK.GUI.Menu
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using Surgical.SDK.EventData;

    public class MenuColorBool : MenuColor, IMenuValue<bool>
    {
        private bool value;

        private Size2 size;

        public MenuColorBool(string id, Color defaultColor, bool defaultBool) : base(id, ColorBoolToJObject(defaultColor, defaultBool))
        {

        }

        public new bool Value
        {
            get => this.value;
            set => this.value ^= this.value != value && this.UpdateValue(value);
        }

        protected override Size2 GetSize()
        {
            return AddButton(base.GetSize(), out this.size);
        }

        protected internal override bool InsideExpandableArea(Point point, int width)
        {
            return base.InsideExpandableArea(point, width - this.size.Width);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            width -= this.size.Width;
            base.OnWndProc(point, width, args);
            point.X += width;

            this.Value ^= Menu.IsLeftClick(args.Message) && Menu.IsCursorInside(point, this.size);
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawBox(point, this.size, this.Value ? Color.Green : Color.Red);
            Theme.DrawBorders(point, this.size);
        }

        protected override JToken Token
        {
            get => ColorBoolToJObject(base.Token, this.Value);
            set
            {
                var (token, @bool) = JObjectToColorBool(value);

                base.Token = token;
                this.value = @bool;
            }
        }

        private static JObject ColorBoolToJObject(Color color, bool @bool)
        {
            return ColorBoolToJObject(new JArray { color.R, color.G, color.B, color.A }, @bool);
        }

        private static JObject ColorBoolToJObject(JToken token, bool @bool)
        {
            return new JObject { { "Color", token }, { "Bool", @bool } };
        }

        private static (JArray Color, bool Bool) JObjectToColorBool(JToken o)
        {
            return (Color: o["Color"]!.Value<JArray>(), Bool: o["Bool"]!.Value<bool>());
        }
    }
}