namespace SparkTech.SDK.GUI.Menu.Items
{
    using System.Drawing;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Game;

    public class MenuColorBool : MenuColor, IMenuValue<bool>
    {
        public MenuColorBool(string id, Color defaultColor, bool defaultBool) : base(id, ColorBoolToJObject(defaultColor, defaultBool))
        {

        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            width -= this.buttonSize.Width;

            base.OnWndProc(point, width, args);

            point.X += width;

            this.Value ^= Menu.IsCursorInside(point, this.buttonSize) && Menu.IsLeftClick(args.Message);
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.buttonSize.Width;

            base.OnEndScene(point, width);

            point.X += width;

            Theme.DrawBox(this.Value ? SharpDX.Color.Green : SharpDX.Color.Red, point, this.buttonSize);
        }

        private Size buttonSize;

        protected override Size GetSize()
        {
            var s = base.GetSize();
            s.Width += s.Height;

            this.buttonSize = new Size(28, s.Height);

            return s;
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

        private bool value;

        public new bool Value
        {
            get => this.value;
            set => this.value ^= this.value != value && this.UpdateValue(value);
        }

        private static JObject ColorBoolToJObject(Color color, bool @bool)
        {
            return new JObject { { "Bool", @bool }, { "Color", ColorToJArray(color) } };
        }

        private static (Color Color, bool Bool) JObjectToColorBool(JToken o)
        {
            return (Color: JArrayToColor(o["Color"].Value<JArray>()), Bool: o["Bool"].Value<bool>());
        }
    }
}