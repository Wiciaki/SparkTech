namespace Surgical.SDK.Rendering
{
    using System;
    using System.Linq;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Surgical.SDK.API;
    using Surgical.SDK.Logging;

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
            Fragment.SetRenderTarget = () => InvokeRenderEvent(OnSetRenderTarget);

            static void OnExit(object o, EventArgs args) => InvokeRenderEvent(OnDispose);

            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            typeof(Vector).Trigger();
            typeof(Circle).Trigger();
            typeof(Text).Trigger();
            typeof(Picture).Trigger();
        }

        public static Device Device => Fragment.Device;

        public static event Action? OnDraw, OnBeginScene, OnEndScene, OnLostDevice, OnResetDevice, OnDispose, OnSetRenderTarget;

        public static Size2 Resolution => Fragment.Resolution;

        private static void InvokeRenderEvent(Action? evt)
        {
            if (evt == null)
            {
                return;
            }

            foreach (var callback in evt.GetInvocationList().Cast<Action>())
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