namespace SparkTech.SDK.Rendering
{
    using System;
    using System.Runtime.CompilerServices;

    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.API.Fragments;

    public static class Render
    {
        public static Device Device => r.Device;

        public static event Action OnDraw, OnBeginScene, OnEndScene, OnLostDevice, OnResetDevice, OnDispose, OnResolutionChanged;

        public static Size2 Resolution()
        {
            return r.Resolution();
        }

        public static Matrix ProjectionMatrix()
        {
            return r.ProjectionMatrix();
        }

        public static Matrix ViewMatrix()
        {
            return r.ViewMatrix();
        }

        private static IRender r;

        internal static void Initialize(IRender render)
        {
            r = render;

            r.Draw = () => OnDraw.SafeInvoke();
            r.BeginScene = () => OnBeginScene.SafeInvoke();
            r.EndScene = () => OnEndScene.SafeInvoke();
            r.LostDevice = () => OnLostDevice.SafeInvoke();
            r.ResetDevice = () => OnResetDevice.SafeInvoke();

            static void OnExit(object o, EventArgs args) => OnDispose.SafeInvoke();

            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            static void Init(Type type) => RuntimeHelpers.RunClassConstructor(type.TypeHandle);

            Init(typeof(Vector));
            Init(typeof(Circle));
            Init(typeof(Text));
            Init(typeof(Image));
        }
    }
}