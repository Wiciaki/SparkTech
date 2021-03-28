namespace SparkTech.Ensoul
{
    using System;

    using EnsoulSharp;

    using SharpDX;
    using SharpDX.Direct3D9;

    using SDK.API;

    internal class RenderAPI : IRenderAPI
    {
        public RenderAPI()
        {
            Drawing.OnBeginScene += args => this.BeginScene();
            Drawing.OnDraw += args => this.Draw();
            Drawing.OnEndScene += args => this.EndScene();
            Drawing.OnPreReset += args => this.LostDevice();
            Drawing.OnPostReset += args => this.ResetDevice();
        }

        public Size2 Resolution => new Size2(Drawing.Width, Drawing.Height);

        public Device Device => Drawing.Direct3DDevice;

        public Action BeginScene { get; set; }

        public Action Draw { get; set; }

        public Action EndScene { get; set; }

        public Action LostDevice { get; set; }

        public Action ResetDevice { get; set; }
    }
}
