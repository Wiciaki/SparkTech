namespace SparkTech.SDK.GUI.Menu
{
    using System;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.EventData;
    using SparkTech.SDK.Properties;
    using SparkTech.SDK.Rendering;

    public class MenuColor : MenuValue, IExpandable, IMenuValue<Color>
    {
        private const string PaletteText = "🎨";

        private Color value;

        private Size2 paletteSize, buttonSize;

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
            get => this.value;
            set
            {
                if (value != this.value && this.UpdateValue(value))
                {
                    this.value = value;
                }
            }
        }

        public bool IsExpanded { get; set; }

        protected override Size2 GetSize()
        {
            return AddButton(AddButton(base.GetSize(), out this.paletteSize, PaletteText), out this.buttonSize);
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.paletteSize.Width + this.buttonSize.Width;
            base.OnEndScene(point, width);
            point.X += width;

            var bgcolor = Theme.BackgroundColor;

            if (this.IsExpanded)
            {
                bgcolor.A = byte.MaxValue;
            }

            Theme.DrawTextBox(point, this.paletteSize, bgcolor, PaletteText, true);

            point.X += this.paletteSize.Width;

            Theme.DrawBox(point, this.buttonSize, this.Value);
            Theme.DrawBorders(point, this.buttonSize);

            if (this.IsExpanded)
            {
                ColorPicker.Draw();
            }
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            if (Menu.IsLeftClick(args.Message) && this.IsExpanded && ColorPicker.IsColorSelected(out var color))
            {
                this.Value = color;
            }
        }

        protected internal override bool InsideExpandableArea(Point point, int width)
        {
            return Menu.IsCursorInside(point, new Size2(width, this.Size.Height));
        }

        #endregion

        #region Properties

        protected override JToken Token
        {
            get => ColorToJArray(this.value);
            set => this.value = JArrayToColor((JArray)value);
        }

        #endregion

        #region Methods

        private static Color JArrayToColor(JArray array)
        {
            return new Color(Enumerable.Range(0, 4).Select(i => array[i].Value<byte>()).ToArray());
        }

        private static JArray ColorToJArray(Color color)
        {
            return new JArray { color.R, color.G, color.B, color.A };
        }

        #endregion

        private static class ColorPicker
        {
            private const float BorderThickness = 4f;

            private static readonly Texture Texture;

            private static readonly Size2 Size;

            private static readonly int Pitch;

            private static readonly byte[] PixelData;

            static ColorPicker()
            {
                Size = new Size2(600, 388);
                Texture = Texture.FromMemory(Render.Device, Resources.Picker, Size.Width, Size.Height, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);

                var colorsCount = Size.Width * Size.Height * 4;

                using (var surface = Texture.GetSurfaceLevel(0))
                {
                    var rectangle = surface.LockRectangle(LockFlags.ReadOnly);
                    var pointer = new DataPointer(rectangle.DataPointer, colorsCount);

                    Pitch = rectangle.Pitch;
                    PixelData = pointer.ToArray();

                    surface.UnlockRectangle();
                }
            }

            private static Point GetPickerPosition()
            {
                return new Point(Render.Width - 50 - Size.Width, Render.Height - 330 - Size.Height);
            }

            private static Color GetPixel(int x, int y)
            {
                var result = new byte[4];
                var index = x * 4 + y * Pitch;
                Array.Copy(PixelData, index, result, 0, 4);

                return new ColorBGRA(result);
            }

            public static bool IsColorSelected(out Color color)
            {
                var palette = GetPickerPosition();

                if (Menu.IsCursorInside(palette, Size))
                {
                    var cursor = UserInput.Cursor2D;

                    color = GetPixel(cursor.X - palette.X, cursor.Y - palette.Y);
                    return true;
                }

                color = default(Color);
                return false;
            }

            public static void Draw()
            {
                var point = GetPickerPosition();

                Picture.Draw(point, Texture);

                var v1 = new Vector2(point.X, point.Y);
                var v2 = new Vector2(point.X + Size.Width, point.Y);
                var v3 = new Vector2(point.X + Size.Width, point.Y + Size.Height);
                var v4 = new Vector2(point.X, point.Y + Size.Height);

                Vector.Draw(Theme.BorderColor, BorderThickness, v1, v2, v3, v4, v1);
            }
        }
    }
}