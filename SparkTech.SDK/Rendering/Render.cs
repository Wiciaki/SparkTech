namespace SparkTech.SDK.Rendering
{
    using System;

    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.Misc;
    using SparkTech.SDK.Platform.API;

    public static class Render
    {
        public static Device Direct3DDevice { get; private set; }

        public static event Action OnRender;

        public static event Action OnBeginScene;

        public static event Action OnEndScene;

        public static event Action OnLostDevice;
        
        public static event Action OnResetDevice;
        
        public static event Action OnDispose;

        private static void OnDisposeEventHandler(object o, EventArgs args) => OnDispose.SafeInvoke();

        private static IRender r;

        internal static void Initialize(IRender render)
        {
            r = render;

            Direct3DDevice = r.GetDevice();

            r.Render = OnRender.SafeInvoke;
            r.BeginScene = OnBeginScene.SafeInvoke;
            r.EndScene = OnEndScene.SafeInvoke;
            r.LostDevice = OnLostDevice.SafeInvoke;
            r.ResetDevice = OnResetDevice.SafeInvoke;

            AppDomain.CurrentDomain.DomainUnload += OnDisposeEventHandler;
            AppDomain.CurrentDomain.ProcessExit += OnDisposeEventHandler;
        }

        public static int Height() => r.Height();

        public static int Width() => r.Width();

        public static Matrix ProjectionMatrix() => r.ProjectionMatrix();

        public static Matrix ViewMatrix() => r.ViewMatrix();

        public static Vector2 WorldToScreen(this Vector3 pos) => r.WorldToScreen(pos);

        public static Vector2 WorldToMinimap(this Vector3 pos) => r.WorldToMinimap(pos);

        public static Vector3 ScreenToWorld(this Vector3 pos) => r.ScreenToWorld(pos);

        /*public static void DrawLine(Vector2 start, Vector2 end, float thickness, Color color)
        {

        }

        public static void DrawCircle(Vector2 pos, float radius, Color color)
        {

        }

        public static void DrawText(Vector2 pos, string text, int size, Color color)
        {

        }*/
    }
}