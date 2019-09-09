namespace SparkTech.SDK.GUI.Menu
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.EventArgs;

    public class MenuColorBool : MenuColor, IMenuValue<bool>
    {
        public MenuColorBool(string id, Color defaultColor, bool defaultBool) : base(id, ColorBoolToJObject(defaultColor, defaultBool))
        {

        }

        private bool value;

        public new bool Value
        {
            get => this.value;
            set => this.value ^= this.value != value && this.UpdateValue(value);
        }

        private Size2 size;

        protected override Size2 GetSize()
        {
            return AddButton(base.GetSize(), out this.size);
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
            get => ColorBoolToJObject(base.Value, this.Value);
            set
            {
                var (color, b) = JObjectToColorBool(value);

                base.Value = color;
                this.Value = b;
            }
        }

        private static JObject ColorBoolToJObject(Color color, bool @bool)
        {
            return new JObject { { "Color", ColorToJArray(color) }, { "Bool", @bool } };
        }

        private static (Color Color, bool Bool) JObjectToColorBool(JToken o)
        {
            return (Color: JArrayToColor(o["Color"].Value<JArray>()), Bool: o["Bool"].Value<bool>());
        }
    }
}