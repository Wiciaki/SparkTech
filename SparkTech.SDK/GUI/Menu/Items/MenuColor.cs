namespace SparkTech.SDK.GUI.Menu.Items
{
    using Newtonsoft.Json.Linq;
    using System.Drawing;

    using SparkTech.SDK.Misc;

    public class MenuColor : MenuValue, IMenuValue<Color>, IMenuValue<SharpDX.Color>
    {
        #region Fields

        private Color color;

        #endregion

        private bool picking;

        #region Constructors and Destructors

        public MenuColor(string id, Color defaultValue) : base(id, ColorToJArray(defaultValue))
        { }

        #endregion

        #region Public Properties

        public Color Value
        {
            get => this.color;
            set
            {
                if (value == this.color)
                {
                    return;
                }

                this.color = value;

                this.OnPropertyChanged(nameof(this.Value));
            }
        }

        SharpDX.Color IMenuValue<SharpDX.Color>.Value
        {
            get => this.Value.ToSharpDXColor();
            set => this.Value = value.ToSystemColor();
        }

        private Size buttonSize;

        protected override Size GetSize()
        {
            var size = base.GetSize();

            this.buttonSize = new Size(size.Height, size.Height);

            size.Width += size.Height;

            return size;
        }

        protected internal override void OnEndScene(Point point, int groupWidth)
        {
            size.Width -= this.buttonSize.Width;

            base.OnEndScene(point, size);

            point.X += size.Width;

            Theme.Draw(new DrawData(point, this.buttonSize) { BackgroundColor = this.color });

            if (!this.picking)
            {
                return;
            }


        }

        protected internal override void OnWndProc(Point point, Size size, GameWndProcEventArgs args)
        {
            point.X += size.Width - this.buttonSize.Width;

            if (args.Message.IsLeftClick() && Mouse.IsInside(point, this.buttonSize))
            {
                this.picking = true;
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

        private static Color JArrayToColor(JArray array)
        {
            var a = array[0].Value<byte>();
            var r = array[1].Value<byte>();
            var g = array[2].Value<byte>();
            var b = array[3].Value<byte>();

            return Color.FromArgb(a, r, g, b);
        }

        private static JArray ColorToJArray(Color color)
        {
            return new JArray { color.A, color.R, color.G, color.B };
        }

        #endregion
    }
}
