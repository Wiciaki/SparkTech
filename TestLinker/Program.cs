namespace TestLinker
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using SharpDX;
    using SharpDX.Direct3D9;
    using SharpDX.Windows;

    using Surgical.SDK.API;
    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.Rendering;

    using TestLinker.Properties;

    using Color = SharpDX.Color;

    /// <summary>
    ///   Direct3D9 Font Sample
    /// </summary>
    static class Program
    {
        private static Device device;

        [STAThread]
        static void Main()
        {
            var form = new HookedForm { StartPosition = FormStartPosition.Manual, Left = 0, Top = 0 };

            var width = form.ClientSize.Width;
            var height = form.ClientSize.Height;

            device = new Device(new Direct3D(), 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(width, height) { PresentationInterval = PresentInterval.One });
            
            var bytes = (byte[])new ImageConverter().ConvertTo(Resources.league, typeof(byte[]));
            var texture = Texture.FromMemory(device, bytes, 1910, 1082, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);

            var platform = Platform.Declare("Test");

            platform.Render = form;
            platform.Theme = new AlphaStarTheme();

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
                device.Clear(ClearFlags.Target, Color.Transparent, 1.0f, 0);
                
                device.BeginScene();
                form.BeginScene();

                Picture.Draw(default, texture);
                
                // Vector.Draw(Color.White, 50f, new Vector2(100, 100), new Vector2(150, 150));
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

        private class HookedForm : RenderForm, IRender
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

            public Device Device => device;

            public Action BeginScene { get; set; }

            public Action Draw { get; set; }

            public Action EndScene { get; set; }

            public Action LostDevice { get; set; }

            public Action ResetDevice { get; set; }

            public Action SetRenderTarget { get; set; }

            public Matrix Projection => throw new NotImplementedException();

            public Matrix View => throw new NotImplementedException();
        }
    }
}