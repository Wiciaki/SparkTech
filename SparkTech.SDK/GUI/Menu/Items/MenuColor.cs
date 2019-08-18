namespace SparkTech.SDK.GUI.Menu.Items
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    public class MenuColor : MenuValue, IMenuValue<Color>
    {
        private Color color;

        private bool picking;

        private Size2 buttonSize;

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

        protected override Size2 GetSize()
        {
            var size = base.GetSize();

            this.buttonSize = new Size2(28, size.Height);

            size.Width += size.Height;

            return size;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.buttonSize.Width;

            base.OnEndScene(point, width);

            point.X += width;

            Theme.DrawBox(point, this.buttonSize, this.GetValue<Color>());

            if (!this.picking)
            {
                return;
            }

            point.X += this.buttonSize.Width + Theme.ItemGroupDistance + 20;

            // todo this is temp
            Rendering.Text.Draw("You dont know how hard it's to make a color picker ffs", Color.White, point);
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
            var r = array[0].Value<byte>();
            var g = array[1].Value<byte>();
            var b = array[2].Value<byte>();
            var a = array[3].Value<byte>();
            
            return new Color(r, g, b, a);
        }

        protected static JArray ColorToJArray(Color color)
        {
            return new JArray { color.R, color.G, color.B, color.A };
        }

        #endregion
    }
}
