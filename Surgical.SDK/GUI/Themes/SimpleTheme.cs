namespace Surgical.SDK.GUI.Themes
{
    using SharpDX;
    using SharpDX.Direct3D9;

    public class SimpleTheme : DefaultTheme
    {
        public SimpleTheme(Color textColor, Color bgColor)
        {
            this.TextColor = textColor;
            this.BorderColor = textColor;

            var w = bgColor;
            w.A -= 20;

            this.BackgroundColor = w;
        }

        public override Color TextColor { get; }

        public override Color BorderColor { get; }

        public override Color BackgroundColor { get; }

        public override FontDescription GetFontDescription()
        {
            return new FontDescription { FaceName = "Arial", Height = 16 };
        }
    }
}