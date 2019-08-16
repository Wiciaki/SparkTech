namespace SparkTech.SDK.GUI.Menu.Items
{
    using System;
    using System.Drawing;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Misc;

    using Color = SharpDX.Color;

    public class MenuButton : MenuText
    {
        public Action OnPress { get; set; }

        public MenuButton(string id) : base(id)
        {

        }

        private bool pressing;

        private Size size;

        protected override Size GetSize()
        {
            var s = base.GetSize();
            s.Width += 28;

            this.size = new Size(28, s.Height);

            return s;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;

            base.OnEndScene(point, width);

            point.X += width;

            Theme.DrawBox(this.pressing ? Color.Gray : Color.DarkGray, point, this.size);
        }

        protected internal override void OnWndProc(Point point, int width, WndProcEventArgs args)
        {
            point.X += width - this.size.Width;

            if (!Menu.IsCursorInside(point, this.size))
            {
                this.pressing = false;
                return;
            }

            if (Menu.IsLeftClick(args.Message))
            {
                this.pressing = true;
                return;
            }

            if (!this.pressing || args.Message != WindowsMessages.LBUTTONUP)
            {
                return;
            }

            this.pressing = false;

            this.OnPress.SafeInvoke();
        }
    }
}