namespace SparkTech.SDK.Rendering
{
    using System;
    using System.Linq;

    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.API;
    using SparkTech.SDK.Logging;

    public static class Render
    {
        private static readonly IRenderAPI Fragment;

        static Render()
        {
            Fragment = Platform.RenderFragment ?? throw Platform.APIException("RenderAPI");

            Fragment.Draw = () => InvokeRenderEvent(OnDraw);
            Fragment.BeginScene = () => InvokeRenderEvent(OnBeginScene);
            Fragment.EndScene = () => InvokeRenderEvent(OnEndScene);
            Fragment.LostDevice = () => InvokeRenderEvent(OnLostDevice);
            Fragment.ResetDevice = () => InvokeRenderEvent(OnResetDevice);

            void OnExit(object o, EventArgs args) => InvokeRenderEvent(OnDispose);

            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            typeof(Vector).Trigger();
            typeof(Circle).Trigger();
            typeof(Text).Trigger();
            typeof(Picture).Trigger();
        }

        public static Device Device => Fragment.Device;

        public static event Action OnDraw, OnBeginScene, OnEndScene, OnLostDevice, OnResetDevice, OnDispose;

        public static int Width => Fragment.Width;

        public static int Height => Fragment.Height;

        public static Size2 Resolution => new Size2(Width, Height);

        private static void InvokeRenderEvent(Action @event)
        {
            if (@event == null || !Platform.IsLoaded)
            {
                return;
            }

            foreach (var callback in @event.GetInvocationList().Cast<Action>())
            {
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }
    }
}