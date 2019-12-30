namespace GUIEditor
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using GUIEditor.Properties;

    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Windows;

    using Surgical.SDK;
    using Surgical.SDK.API;
    using Surgical.SDK.EventData;
    using Surgical.SDK.Licensing;
    using Surgical.SDK.Rendering;

    using Color = SharpDX.Color;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            var form = new MainForm();

            var width = form.ClientSize.Width;
            var height = form.ClientSize.Height;

            form.Device = new Device(new Direct3D(), 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(width, height) { PresentationInterval = PresentInterval.One });

            const string FileName = "background.png";

            var size = Image.FromFile(FileName).Size;
            var texture = Texture.FromFile(form.Device, FileName, size.Width, size.Height, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);

            var platform = Platform.Declare("GUI Editor");

            platform.RenderAPI = form;
            platform.UserInputAPI = form;
            platform.AuthResult = new AuthResult(true, DateTime.Now.AddDays(2));

            platform.Boot();

            RenderLoop.Run(form, () =>
            {
                form.Device.Clear(ClearFlags.Target, Color.Transparent, 1.0f, 0);
                form.Device.BeginScene();

                form.BeginScene();

                Picture.Draw(default, texture);

                form.Draw();

                var cursor = UserInput.CursorPosition;
                Vector.Draw(Color.WhiteSmoke, 6, new Vector2(cursor.X - 3, cursor.Y), new Vector2(cursor.X + 3, cursor.Y));

                form.EndScene();

                form.Device.EndScene();
                form.Device.Present();
            });
        }

        private class MainForm : RenderForm, IRenderAPI, IUserInputAPI
        {
            public MainForm() : base("Surgical.SDK - GUI Editor")
            {
                this.AllowUserResizing = false;
                this.Size = new Size(1920, 1080);
                this.Icon = Resources.Surgeon;
            }

            public Size2 Resolution()
            {
                return new Size2(1920, 1080);
            }

            protected override void WndProc(ref Message m)
            {
                var message = (WindowsMessages)m.Msg;
                var key = (Key)m.WParam.ToInt64();

                var args = new WndProcEventArgs(message, key);

                if (!this.WndProcBlock(args))
                {
                    base.WndProc(ref m);
                }
            }

            public Device Device { get; set; }

            public Action BeginScene { get; set; }

            public Action Draw { get; set; }

            public Action EndScene { get; set; }

            public Action LostDevice { get; set; }

            public Action ResetDevice { get; set; }

            public Action SetRenderTarget { get; set; }

            public Action<WndProcEventArgs> WndProcess { get; set; }

            public Vector2 CursorPosition
            {
                get
                {
                    var cursor = Cursor.Position;
                    var offset = this.DesktopLocation;

                    var x = cursor.X - offset.X - 8;
                    var y = cursor.Y - offset.Y - 30;

                    return new Vector2(x, y);
                }
            }

            private bool WndProcBlock(WndProcEventArgs args)
            {
                if (this.WndProcess == null)
                {
                    return false;
                }

                this.WndProcess(args);

                return args.IsBlocked;
            }
        }
    }
}