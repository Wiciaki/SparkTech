namespace Surgical.SDK.Rendering
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.Logging;

    public static class Render
    {
        public static Device Device => render.Device;

        public static event Action OnDraw, OnBeginScene, OnEndScene, OnLostDevice, OnResetDevice, OnDispose, OnSetRenderTarget;

        public static Size2 Resolution()
        {
            return render.Resolution();
        }

        public static Matrix Projection => render.Projection;

        public static Matrix View => render.View;

        private static IRender render;

        internal static void Initialize(IRender r)
        {
            render = r;

            r.Draw = () => InvokeRenderEvent(OnDraw);
            r.BeginScene = () => InvokeRenderEvent(OnBeginScene);
            r.EndScene = () => InvokeRenderEvent(OnEndScene);
            r.LostDevice = () => InvokeRenderEvent(OnLostDevice);
            r.ResetDevice = () => InvokeRenderEvent(OnResetDevice);
            r.SetRenderTarget = () => InvokeRenderEvent(OnSetRenderTarget);

            static void OnExit(object o, EventArgs args) => InvokeRenderEvent(OnDispose);

            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            static void Init(Type type) => RuntimeHelpers.RunClassConstructor(type.TypeHandle);

            Init(typeof(Vector));
            Init(typeof(Circle));
            Init(typeof(Text));
            Init(typeof(Picture));
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