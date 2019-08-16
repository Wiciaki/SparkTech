namespace SparkTech.SDK.GUI.Menu.Items
{
    using Newtonsoft.Json.Linq;
    using System.Drawing;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Misc;

    public class MenuColor : MenuValue, IMenuValue<Color>, IMenuValue<SharpDX.Color>
    {
        private Color color;

        private bool picking;

        private Size buttonSize;

        #region Constructors and Destructors

        public MenuColor(string id, Color defaultValue) : this(id, ColorToJArray(defaultValue))
        { }

        protected MenuColor(string id, JToken defaultValue) : base(id, defaultValue)
        { }

        #endregion

        #region Public Properties

        public Color Value
        {
            get => this.color;
            set => this.color = value != this.color && this.UpdateValue(value) ? value : this.color;
        }

        SharpDX.Color IMenuValue<SharpDX.Color>.Value
        {
            get => this.Value.ToSharpDXColor();
            set => this.Value = value.ToSystemColor();
        }

        protected override Size GetSize()
        {
            var size = base.GetSize();

            this.buttonSize = new Size(28, size.Height);

            size.Width += size.Height;

            return size;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.buttonSize.Width;

            base.OnEndScene(point, width);

            point.X += width;

            Theme.DrawBox(this.GetValue<SharpDX.Color>(), point, this.buttonSize);

            if (!this.picking)
            {
                return;
            }

            point.X += this.buttonSize.Width + Theme.ItemGroupDistance + 20;

            // todo this is temp
            Rendering.Text.Draw("You dont know how hard it's to make a color picker ffs", SharpDX.Color.White, point);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            point.X += width - this.buttonSize.Width;

            if (Menu.IsLeftClick(args.Message) && Menu.IsCursorInside(point, this.buttonSize))
            {
                this.picking ^= true;
            }
        }

        #endregion

        #region Properties

        protected override JToken Token
        {
            get => ColorToJArray(this.color);
            set => this.color = JArrayToColor((JArray)value);
        }

        #endregion

        #region Methods

        protected static Color JArrayToColor(JArray array)
        {
            var a = array[0].Value<byte>();
            var r = array[1].Value<byte>();
            var g = array[2].Value<byte>();
            var b = array[3].Value<byte>();

            return Color.FromArgb(a, r, g, b);
        }

        protected static JArray ColorToJArray(Color color)
        {
            return new JArray { color.A, color.R, color.G, color.B };
        }

        #endregion
    }
}
