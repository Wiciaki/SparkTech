namespace TestLinker
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Windows;

    using Surgical.SDK;
    using Surgical.SDK.API;
    using Surgical.SDK.EventData;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Rendering;

    using TestLinker.Properties;

    using Color = SharpDX.Color;

    internal static class Program
    {
        private static Device device;

        [STAThread]
        private static void Main()
        {
            var form = new HookedForm { StartPosition = FormStartPosition.Manual, Left = 0, Top = 0, AllowUserResizing = false };

            var width = form.ClientSize.Width;
            var height = form.ClientSize.Height;

            device = new Device(new Direct3D(), 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(width, height) { PresentationInterval = PresentInterval.One });
            
            var bytes = (byte[])new ImageConverter().ConvertTo(Resources.league, typeof(byte[]));
            var texture = Texture.FromMemory(device, bytes, 1910, 1082, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);

            var platform = Platform.Declare("Test");

            platform.Render = form;
            platform.WndProc = form;
            //platform.Theme = new AlphaStarTheme();

            platform.Boot();

            RenderLoop.Run(form, () =>
            {
                device.Clear(ClearFlags.Target, Color.Transparent, 1.0f, 0);
                device.BeginScene();

                form.BeginScene();
                Picture.Draw(default, texture);
                form.Draw();
                form.EndScene();

                device.EndScene();
                device.Present();
            });
        }

        private class HookedForm : RenderForm, IRender, IWndProc
        {
            public HookedForm() : base("SharpDX - Render test (Surgeon)")
            {
                this.Size = new Size(1920, 1080);

                //this.FormBorderStyle = FormBorderStyle.None;
            }

            public Size2 Resolution()
            {
                return new Size2(1920, 1080);
            }

            protected override void WndProc(ref Message m)
            {
                var message = (WindowsMessages)m.Msg;
                var keys = (Surgical.SDK.Keys)m.WParam;

                var args = new WndProcEventArgs(message, keys);

                if (!this.WndProcBlock(args))
                {
                    base.WndProc(ref m);
                }
            }

            public Device Device => device;

            public Action BeginScene { get; set; }

            public Action Draw { get; set; }

            public Action EndScene { get; set; }

            public Action LostDevice { get; set; }

            public Action ResetDevice { get; set; }

            public Action SetRenderTarget { get; set; }

            public Matrix Projection => throw new NotImplementedException();

            public Matrix View => throw new NotImplementedException();

            Action<WndProcEventArgs> IWndProc.WndProc { get; set; }

            public Vector2 CursorPosition => new Vector2(Cursor.Position.X, Cursor.Position.Y);

            private bool WndProcBlock(WndProcEventArgs args)
            {
                var @this = (IWndProc)this;
                var wndProc = @this.WndProc;

                if (wndProc == null)
                {
                    return false;
                }

                wndProc(args);

                return args.IsBlocked;
            }
        }
    }
}