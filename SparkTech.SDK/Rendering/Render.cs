namespace SparkTech.SDK.Rendering
{
    using System;
    using System.Runtime.CompilerServices;

    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.Platform.API;

    public static class Render
    {
        public static Device Direct3DDevice { get; private set; }

        public static event Action OnDraw, OnBeginScene, OnEndScene, OnLostDevice, OnResetDevice, OnDispose, OnResolutionChanged;

        public static Size2 Resolution() => api.Resolution();

        public static Matrix ProjectionMatrix() => api.ProjectionMatrix();

        public static Matrix ViewMatrix() => api.ViewMatrix();

        public static Vector2 WorldToScreen(this Vector3 pos) => api.WorldToScreen(pos);

        public static Vector2 WorldToMinimap(this Vector3 pos) => api.WorldToMinimap(pos);

        public static Vector3 ScreenToWorld(this Vector3 pos) => api.ScreenToWorld(pos);

        private static IRender api;

        internal static void Initialize(IRender render)
        {
            api = render;

            Direct3DDevice = api.GetDevice();

            api.Draw = () => OnDraw.SafeInvoke();
            api.BeginScene = () => OnBeginScene.SafeInvoke();
            api.EndScene = () => OnEndScene.SafeInvoke();
            api.LostDevice = OnLostDevice.SafeInvoke;
            api.ResetDevice = OnResetDevice.SafeInvoke;

            static void OnExit(object o, EventArgs args) => OnDispose.SafeInvoke();

            AppDomain.CurrentDomain.DomainUnload += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            var triggerable = new[] { typeof(Vector), typeof(Circle), typeof(Text), typeof(Image) };

            foreach (var type in triggerable)
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
        }
    }
}