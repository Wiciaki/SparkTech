namespace SparkTech.SDK.GUI.Menu
{
    using System;

    using SharpDX;

    using SparkTech.SDK.EventData;
    using SparkTech.SDK.Logging;

    public class MenuAction : MenuText
    {
        private readonly Action action;

        private bool pressing;

        private Size2 size;

        public MenuAction(string id, Action action) : base(id)
        {
            this.action = action;
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

            Theme.DrawBox(point, this.size, this.pressing ? Color.Gray : Color.DarkGray);
            Theme.DrawBorders(point, this.size);
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

            try
            {
                this.action();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}