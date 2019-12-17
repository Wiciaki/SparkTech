namespace Surgical.SDK.GUI.Menu
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    using Surgical.SDK.EventData;

    public class MenuColor : MenuValue, IMenuValue<Color>
    {
        protected Color tmValue;

        private bool picking;

        private Size2 size;

        #region Constructors and Destructors

        public MenuColor(string id, Color defaultValue) : this(id, ColorToJArray(defaultValue))
        { }

        protected MenuColor(string id, JToken defaultValue) : base(id, defaultValue)
        { }

        #endregion

        #region Public Properties

        public Color Value
        {
            get => this.tmValue;
            set
            {
                if (value != this.tmValue && this.UpdateValue(value))
                {
                    this.tmValue = value;
                }
            }
        }

        protected override Size2 GetSize()
        {
            return AddButton(AddButton(base.GetSize(), out this.size, "🎨"), out _);
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width * 2;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawTextBox(point, this.size, "🎨", true);

            point.X += this.size.Width;

            Theme.DrawBox(point, this.size, this.Value);
            Theme.DrawBorders(point, this.size);

            if (!this.picking)
            {
                return;
            }

            point.X += this.size.Width;

            // todo this is temp
            Rendering.Text.Draw("You dont know how hard it's to make a color picker ffs", Color.White, point);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            point.X += width - this.size.Width;

            if (Menu.IsLeftClick(args.Message) && Menu.IsCursorInside(point, this.size))
            {
                this.picking ^= true;
            }
        }

        #endregion

        #region Properties

        protected override JToken Token
        {
            get => ColorToJArray(this.tmValue);
            set => this.tmValue = JArrayToColor((JArray)value);
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
