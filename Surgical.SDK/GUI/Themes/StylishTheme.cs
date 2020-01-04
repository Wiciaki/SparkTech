namespace Surgical.SDK.GUI.Themes
{
    using SharpDX;
    using SharpDX.Direct3D9;

    using Surgical.SDK.Rendering;

    public class StylishTheme : DefaultTheme
    {
        protected StylishTheme(Color bgcolor) => this.BackgroundColor = bgcolor;

        public override Color BackgroundColor { get; }

        public override FontDescription GetFontDescription()
        {
            var description = base.GetFontDescription();
            description.FaceName = "Cambria";

            return description;
        }

        public override void DrawBorders(Point point, Color color, params Size2[] sizes)
        {
            foreach (var size in sizes)
            {
                var p = new[] { new Vector2(point.X + size.Width, point.Y), new Vector2(point.X, point.Y), new Vector2(point.X, point.Y + size.Height) };

                Vector.Draw(color, 2f, p);

                point.Y += size.Height;
            }
        }

        public static ITheme CreateRed()
        {
            var bgcolor = Color.DarkRed;
            bgcolor.A -= 100;

            return new StylishTheme(bgcolor);
        }

        public static ITheme CreateBlue()
        {
            var bgcolor = Color.DarkBlue;
            bgcolor.A -= 100;

            return new StylishTheme(bgcolor);
        }
    }
}