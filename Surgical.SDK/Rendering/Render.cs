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
        public static Device Device => fragment.Device;

        public static event Action OnDraw, OnBeginScene, OnEndScene, OnLostDevice, OnResetDevice, OnDispose, OnSetRenderTarget;

        public static Size2 Resolution()
        {
            return fragment.Resolution();
        }

        private static IRenderAPI fragment;

        internal static void Initialize(IRenderAPI api)
        {
            fragment = api;

            fragment.Draw = () => InvokeRenderEvent(OnDraw);
            fragment.BeginScene = () => InvokeRenderEvent(OnBeginScene);
            fragment.EndScene = () => InvokeRenderEvent(OnEndScene);
            fragment.LostDevice = () => InvokeRenderEvent(OnLostDevice);
            fragment.ResetDevice = () => InvokeRenderEvent(OnResetDevice);
            fragment.SetRenderTarget = () => InvokeRenderEvent(OnSetRenderTarget);

            static void OnExit(object o, EventArgs args) => InvokeRenderEvent(OnDispose);

            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            typeof(Vector).Trigger();
            typeof(Circle).Trigger();
            typeof(Text).Trigger();
            typeof(Picture).Trigger();
        }

        private static void InvokeRenderEvent(Action e)
        {
            if (e == null)
            {
                return;
            }

            foreach (var callback in e.GetInvocationList().Cast<Action>())
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