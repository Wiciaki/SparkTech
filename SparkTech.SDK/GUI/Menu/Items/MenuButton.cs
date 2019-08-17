namespace SparkTech.SDK.GUI.Menu.Items
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Misc;

    public class MenuButton : MenuText
    {
        public Action OnPress { get; set; }

        public MenuButton(string id) : base(id)
        {

        }

        private bool pressing;

        private Size2 size;
        
        protected override Size2 GetSize()
        {
            var s = base.GetSize();
            s.Width += 28;

            this.size = new Size2(28, s.Height);

            return s;
        }

        protected internal override void OnEndScene(Point point, int width)
        {
            width -= this.size.Width;

            base.OnEndScene(point, width);

            point.X += width;

            Theme.DrawBox(point, this.size, this.pressing ? Color.Gray : Color.DarkGray);
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