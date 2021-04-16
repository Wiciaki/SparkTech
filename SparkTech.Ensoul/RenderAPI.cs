namespace SparkTech.Ensoul
{
    using System;

    using EnsoulSharp;

    using SharpDX;
    using SharpDX.Direct3D9;

    using SDK.API;

    internal class RenderAPI : IRenderAPI
    {
        public int Width => Drawing.Width;

        public int Height => Drawing.Height;

        public Action Draw { get; set; }

        public Action EndScene { get; set; }

        public Action LostDevice { get; set; }

        public Action ResetDevice { get; set; }

        public Device Device => Drawing.Direct3DDevice;

        public RenderAPI()
        {
            Drawing.OnDraw += args =>
            {
                EnsoulSharp.SDK.MenuUI.MenuManager.Instance.MenuVisible = false;
                this.Draw();
            };

            Drawing.OnEndScene += args => this.EndScene();
            Drawing.OnPreReset += args => this.LostDevice();
            Drawing.OnPostReset += args => this.ResetDevice();
        }

        //Hacks.DisableDrawings = true;

        //this.Device = new DeviceEx(new Direct3DEx(), 0, DeviceType.Hardware, EnsoulSharp.Game.Window,
        //    CreateFlags.HardwareVertexProcessing, new PresentParameters(Drawing.Width, Drawing.Height)
        //    { PresentationInterval = PresentInterval.One });

        //Task.Delay(100).ContinueWith(Hook);

        //Drawing.OnDraw += args => {
        //    Vector.Draw(SharpDX.Color.Red, 20, new SharpDX.Vector2(10, 10), new SharpDX.Vector2(20, 20));
        //    this.Draw();
        //};

        //private void Hook(Task _)
        //{
        //    UserInput.OnWndProc += args =>
        //    {
        //        if (args.Message == WindowsMessages.PAINT)
        //        {
        //            this.Render();
        //        }
        //    };
        //}

        //private void Render()
        //{
        //    this.Device.Clear(ClearFlags.Target, Color.Zero, 1f, 0);
        //    this.Device.BeginScene();
        //    this.BeginScene();

        //    Vector.Draw(Color.Red, 20, new Vector2(10, 10), new Vector2(20, 20));

        //    this.Draw();
        //    this.EndScene();

        //    this.Device.EndScene();
        //    this.Device.Present();
        //}
    }
}