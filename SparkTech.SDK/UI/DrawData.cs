namespace SparkTech.SDK.UI
{
    using System.Drawing;

    public struct DrawData
    {
        public readonly Point Point;

        public readonly Size Size;

        public string Text;

        public bool ForceTextCentered, Bold;

        public Color FontColor, BackgroundColor;

        public DrawData(Point point, Size size)
        {
            this.Point = point;

            this.Size = size;

            this.Text = null;

            this.ForceTextCentered = this.Bold = false;

            this.FontColor = Theme.FontColor;

            this.BackgroundColor = Theme.BackgroundColor;
        }
    }
}