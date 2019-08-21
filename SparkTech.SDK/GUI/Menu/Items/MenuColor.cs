﻿namespace SparkTech.SDK.GUI.Menu
{
    using Newtonsoft.Json.Linq;

    using SharpDX;

    public class MenuColor : MenuValue, IMenuValue<Color>
    {
        private Color value;

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
            get => this.value;
            set
            {
                if (value != this.value && this.UpdateValue(value))
                {
                    this.value = value;
                }
            }
        }

        protected override Size2 GetSize()
        {
            return AddButton(base.GetSize(), out this.size);
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;
            base.OnEndScene(point, width);
            point.X += width;

            Theme.DrawBox(point, this.size, this.GetValue<Color>());
            Theme.DrawBorders(point, this.size);

            if (!this.picking)
            {
                return;
            }

            point.X += this.size.Width + Theme.ItemGroupDistance + 20;

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
            get => ColorToJArray(this.value);
            set => this.value = JArrayToColor((JArray)value);
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
