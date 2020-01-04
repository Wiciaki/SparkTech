namespace Surgical.SDK.GUI.Themes
{
    using SharpDX;
    using SharpDX.Direct3D9;

    public class SimpleTheme : DefaultTheme
    {
        protected SimpleTheme(Color color, Color bgcolor)
        {
            this.TextColor = color;
            this.BorderColor = color;
            this.BackgroundColor = bgcolor;
        }

        public override Color TextColor { get; }

        public override Color BorderColor { get; }

        public override Color BackgroundColor { get; }

        public override FontDescription GetFontDescription()
        {
            var description = base.GetFontDescription();

            description.FaceName = "Bariol";
            description.Height = 16;

            return description;
        }

        public static ITheme CreateBlack()
        {
            var bgcolor = Color.Black;
            bgcolor.A -= 20;

            return new SimpleTheme(Color.White, bgcolor);
        }

        public static ITheme CreateWhite()
        {
            var bgcolor = Color.White;
            bgcolor.A -= 20;

            return new SimpleTheme(Color.Black, bgcolor);
        }

        public static ITheme CreateBlue()
        {
            var bgcolor = Color.DarkBlue;
            bgcolor.A -= 100;

            return new SimpleTheme(Color.WhiteSmoke, bgcolor);
        }
    }
}