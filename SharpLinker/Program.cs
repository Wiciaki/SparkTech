namespace SharpLinker
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Windows;

    using SparkTech.SDK;
    using SparkTech.SDK.API;
    using SparkTech.SDK.Platform;
    using SparkTech.SDK.Platform.API;

    using Color = SharpDX.Color;
    using Point = SharpDX.Point;

    /// <summary>
    ///   Direct3D9 Font Sample
    /// </summary>
    static class Program
    {
        private static Device device;

        [STAThread]
        static void Main()
        {
            var form = new HookedForm();

            int width = form.ClientSize.Width;
            int height = form.ClientSize.Height;

            device = new Device(new Direct3D(), 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(width, height) { PresentationInterval = PresentInterval.One });

            var platform = new Platform("Shark") { Render = form };

            platform.Boot();

            //// Initialize the Font
            //FontDescription fontDescription = new FontDescription()
            //{
            //    Height = 72,
            //    Italic = false,
            //    CharacterSet = FontCharacterSet.Ansi,
            //    FaceName = "Arial",
            //    MipLevels = 0,
            //    OutputPrecision = FontPrecision.TrueType,
            //    PitchAndFamily = FontPitchAndFamily.Default,
            //    Quality = FontQuality.ClearType,
            //    Weight = FontWeight.Bold
            //};

            //var font = new Font(device, fontDescription);

            //var displayText = "Soon menu test bik";

            //// Measure the text to display
            //var fontDimension = font.MeasureText(null, displayText, new Rectangle(0, 0, width, height), FontDrawFlags.Center | FontDrawFlags.VerticalCenter);

            //int xDir = 1;
            //int yDir = 1;

            RenderLoop.Run(form, () =>
            {
                device.Clear(ClearFlags.Target, Color.Gray, 1.0f, 0);
                
                device.BeginScene();
                form.BeginScene();

                //Vector.Draw(Color.White, 50f, new Vector2(100, 100), new Vector2(150, 150));
                form.Draw();

                //// Make the text boucing on the screen limits
                //if ((fontDimension.Right + xDir) > width)
                //    xDir = -1;
                //else if ((fontDimension.Left + xDir) <= 0)
                //    xDir = 1;

                //if ((fontDimension.Bottom + yDir) > height)
                //    yDir = -1;
                //else if ((fontDimension.Top + yDir) <= 0)
                //    yDir = 1;

                //fontDimension.Left += (int)xDir;
                //fontDimension.Top += (int)yDir;
                //fontDimension.Bottom += (int)yDir;
                //fontDimension.Right += (int)xDir;

                //// Draw the text
                //font.DrawText(null, displayText, fontDimension, FontDrawFlags.Center | FontDrawFlags.VerticalCenter, Color.White);
                form.EndScene();

                device.EndScene();
                device.Present();
            });
        }

        private class HookedForm : RenderForm, IRender//, IGameEvents
        {
            public HookedForm() : base("SharpDX - Test (Spark)")
            {
                this.Size = new Size(1920, 1080);
            }

            public override System.Drawing.Color BackColor { get; set; } = System.Drawing.Color.Wheat;

            protected override void WndProc(ref Message m)
            {
                var message = (WindowsMessages)m.Msg;
                var wParam = (WindowsMessagesWParam)m.WParam;

                this.OnWndProc?.Invoke(new WndProcEventArgs(message, wParam));

                base.WndProc(ref m);
            }

            public Size2 Resolution()
            {
                return new Size2(1920, 1080);
            }

            public Device GetDevice()
            {
                return device;
            }

            public Action BeginScene { get; set; }

            public Action Draw { get; set; }

            public Action EndScene { get; set; }

            public Action LostDevice { get; set; }

            public Action ResetDevice { get; set; }

            public Vector2 WorldToScreen(Vector3 pos)
            {
                throw new NotImplementedException();
            }

            public Vector2 WorldToMinimap(Vector3 pos)
            {
                throw new NotImplementedException();
            }

            public Vector3 ScreenToWorld(Vector3 pos)
            {
                throw new NotImplementedException();
            }

            public Matrix ProjectionMatrix()
            {
                throw new NotImplementedException();
            }

            public Matrix ViewMatrix()
            {
                throw new NotImplementedException();
            }

            public Action<WndProcEventArgs> OnWndProc { get; set; }

            public Point GetCursorPosition()
            {
                var p = Cursor.Position;

                return new Point(p.X, p.Y);
            }
        }
    }
}
