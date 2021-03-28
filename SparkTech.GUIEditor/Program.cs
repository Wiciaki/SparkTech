namespace GUIEditor
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Windows;

    using SparkTech.SDK;
    using SparkTech.SDK.API;
    using SparkTech.SDK.EventData;
    using SparkTech.SDK.Licensing;
    using SparkTech.SDK.Rendering;

    using SparkTech.GUIEditor.Properties;

    using Color = SharpDX.Color;

    internal static class Program
    {
        private const int WindowX = 1936, WindowY = 1117;

        [STAThread]
        private static void Main()
        {
            var form = new MainForm();
            var formSize = form.ClientSize;

            form.Device = new Device(new Direct3D(), 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(formSize.Width, formSize.Height) { PresentationInterval = PresentInterval.One });

            const string FileName = "Background.png";

            if (!File.Exists(FileName))
            {
                var result = (byte[])new ImageConverter().ConvertTo(Resources.Background, typeof(byte[]));

                File.WriteAllBytes(FileName, result);
            }

            var imageSize = Image.FromFile(FileName).Size;
            var texture = Texture.FromFile(form.Device, FileName, imageSize.Width, imageSize.Height, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);

            var platform = new Platform("GUI Editor")
            {
                AuthResult = AuthResult.GetLifetime(),
                RenderAPI = form,
                UserInputAPI = form
            };

            platform.Load();

            RenderLoop.Run(form, () =>
            {
                form.Device.Clear(ClearFlags.Target, Color.Zero, 1f, 0);
                form.Device.BeginScene();

                form.BeginScene();

                Picture.Draw(default, texture);

                form.Draw();
                form.EndScene();

                form.Device.EndScene();
                form.Device.Present();
            });
        }

        private class MainForm : RenderForm, IRenderAPI, IUserInputAPI
        {
            public MainForm() : base("SparkTech.SDK // GUI Editor")
            {
                this.Resolution = new Size2(WindowX, WindowY);

                this.Size = new Size(WindowX, WindowY);
                this.AllowUserResizing = false;
                this.Icon = Resources.icon_sparktech;
            }

            public Size2 Resolution { get; }

            public Device Device { get; set; }

            public Action BeginScene { get; set; }

            public Action Draw { get; set; }

            public Action EndScene { get; set; }

            public Action LostDevice { get; set; }

            public Action ResetDevice { get; set; }

            public Action SetRenderTarget { get; set; }

            public Action<WndProcEventArgs> WndProcess { get; set; }

            protected override void WndProc(ref Message m)
            {
                var process = this.WndProcess;

                if (process != null)
                {
                    var msg = (uint)m.Msg;
                    var wParam = (uint)m.WParam.ToInt64();
                    var lParam = m.LParam.ToInt32();

                    var args = new WndProcEventArgs(msg, wParam, lParam);
                    process(args);

                    if (args.IsBlocked)
                    {
                        return;
                    }
                }

                base.WndProc(ref m);
            }
        }
    }
}