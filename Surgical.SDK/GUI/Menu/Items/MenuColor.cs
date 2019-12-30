namespace Surgical.SDK.GUI.Menu
{
    using System.Drawing;
    using System.IO;

    using Newtonsoft.Json.Linq;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Surgical.SDK.EventData;
    using Surgical.SDK.Properties;
    using Surgical.SDK.Rendering;

    using Color = SharpDX.Color;
    using Filter = SharpDX.Direct3D9.Filter;
    using Point = SharpDX.Point;

    // todo cancerrr
    public class MenuColor : MenuValue, IMenuValue<Color>
    {
        protected Color tmValue;

        protected int tmExtraWidth;

        private bool picking;

        private Size2 paletteSize, buttonSize;

        private static readonly Bitmap Picker;

        private static readonly Texture PickerTexture;

        private static readonly Size2 BitmapSize;

        static MenuColor()
        {
            using (var stream = new MemoryStream(Resources.Picker))
            {
                Picker = new Bitmap(stream);
            }

            BitmapSize = new Size2(Picker.Size.Width, Picker.Size.Height);

            PickerTexture = Texture.FromMemory(Render.Device, Resources.Picker, BitmapSize.Width, BitmapSize.Height, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
        }

        #region Constructors and Destructors

        public MenuColor(string id, Color defaultValue) : this(id, ColorToJArray(defaultValue))
        {

        }

        protected MenuColor(string id, JToken defaultValue) : base(id, defaultValue)
        {

        }

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
            return AddButton(AddButton(base.GetSize(), out this.paletteSize, "🎨"), out this.buttonSize);
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.paletteSize.Width + this.buttonSize.Width;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawTextBox(point, this.paletteSize, "🎨", true);

            point.X += this.paletteSize.Width;

            Theme.DrawBox(point, this.buttonSize, this.Value);
            Theme.DrawBorders(point, this.buttonSize);

            if (!this.picking)
            {
                return;
            }

            point.X += this.buttonSize.Width + this.tmExtraWidth;

            if (Menu.ArrowsEnabled)
            {
                Menu.DrawArrow(point);

                point.X += Menu.ArrowWidth;
            }

            Picture.Draw(point, PickerTexture);
            Theme.DrawBorders(point, BitmapSize);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            point.X += width - this.buttonSize.Width;

            if (Menu.IsLeftClick(args.Message) && Menu.IsCursorInside(point, this.paletteSize))
            {
                this.picking ^= true;
                return;
            }

            if (!this.picking)
            {
                return;
            }

            point.X += this.paletteSize.Width + this.tmExtraWidth;

            if (Menu.ArrowsEnabled)
            {
                point.X += Menu.ArrowWidth;
            }

            if (Menu.IsLeftClick(args.Message) && Menu.IsCursorInside(point, BitmapSize))
            {
                var cursor = (Point)UserInput.CursorPosition;
                var color = Picker.GetPixel(cursor.X - point.X, cursor.Y - point.Y);

                this.Value = new Color(color.R, color.G, color.B, color.A);
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
