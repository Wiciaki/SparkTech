namespace SparkTech.SDK.Rendering
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.Misc;
    using SparkTech.SDK.Platform.API;

    public static class Render
    {
        public static Device Direct3DDevice { get; private set; }

        public static event Action OnDraw, OnBeginScene, OnEndScene, OnLostDevice, OnResetDevice, OnDispose, OnResolutionChanged;

        public static Size Resolution() => api.Resolution();

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

            api.Draw = OnDraw.SafeInvoke;
            api.BeginScene = OnBeginScene.SafeInvoke;
            api.EndScene = OnEndScene.SafeInvoke;
            api.LostDevice = OnLostDevice.SafeInvoke;
            api.ResetDevice = OnResetDevice.SafeInvoke;
            api.ResolutionChanged = OnResolutionChanged.SafeInvoke;

            static void OnDisposeEventHandler(object o, EventArgs args) => OnDispose.SafeInvoke();

            AppDomain.CurrentDomain.DomainUnload += OnDisposeEventHandler;
            AppDomain.CurrentDomain.ProcessExit += OnDisposeEventHandler;

            var triggerable = new[] { typeof(Vector), typeof(Circle), typeof(Text), typeof(Picture) };

            foreach (var type in triggerable)
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
        }
    }
}