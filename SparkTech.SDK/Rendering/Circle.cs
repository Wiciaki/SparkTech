﻿namespace SparkTech.SDK.Rendering
{
    using SharpDX;
    using SharpDX.Direct3D9;

    using SparkTech.SDK.League;
    using SparkTech.SDK.Properties;

    public static class Circle
    {
        /// <summary>
        ///     The maximum radius a circle can be drawn with
        /// </summary>
        private const float Radius = 20000f;

        private const float AntiAlias = 0.65f;

        private static readonly Effect Effect;

        private static readonly EffectHandle EffectHandle;

        private static readonly VertexBuffer VertexBuffer;

        private static readonly VertexDeclaration VertexDeclaration;

        static Circle()
        {
            VertexBuffer = new VertexBuffer(Render.Device, Utilities.SizeOf<Vector4>() * 3, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

            using (var ds = VertexBuffer.Lock(0, 0, LockFlags.None))
            {
                ds.WriteRange(new[] { new Vector4(-Radius, 0f, -Radius, 1.0f), new Vector4(0f, 0f, Radius, 1.0f), new Vector4(Radius, 0f, -Radius, 1.0f) });
            }

            VertexBuffer.Unlock();

            var vertexElements = new[] { new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0), VertexElement.VertexDeclarationEnd };
            VertexDeclaration = new VertexDeclaration(Render.Device, vertexElements);

            Effect = Effect.FromMemory(Render.Device, Resources.RenderEffectCompiled, ShaderFlags.None);
            EffectHandle = Effect.GetTechnique(0);

            Render.OnLostDevice += Effect.OnLostDevice;
            Render.OnResetDevice += Effect.OnResetDevice;

            Render.OnDispose += () =>
            {
                Effect.Dispose();
                EffectHandle.Dispose();
                VertexBuffer.Dispose();
                VertexDeclaration.Dispose();
            };
        }

        #region Public Methods and Operators

        public static void Draw(Color color, float radius, params Vector3[] worldPositions)
        {
            Draw(color, radius, 1f, worldPositions);
        }

        public static void Draw(Color color, float radius, float thickness, params Vector3[] worldPositions)
        {
            Draw(color, radius, thickness, false, worldPositions);
        }

        public static void Draw(Color color, float radius, float thickness, bool filled, params Vector3[] worldPositions)
        {
            var declaration = Render.Device.VertexDeclaration;

            Effect.Technique = EffectHandle;
            Effect.Begin();

            Render.Device.SetStreamSource(0, VertexBuffer, 0, Utilities.SizeOf<Vector4>());
            Render.Device.VertexDeclaration = VertexDeclaration;

            var mod = Game.ViewMatrix * Game.ProjectionMatrix;
            var col = (Vector4)color;

            foreach (var pos in worldPositions)
            {
                var matrix = Matrix.Translation(pos.X, pos.Z, pos.Y) * mod;

                Effect.BeginPass(0);

                Effect.SetValue("ProjectionMatrix", matrix);
                Effect.SetValue("Color", col);
                Effect.SetValue("Radius", radius);
                Effect.SetValue("Width", thickness);
                Effect.SetValue("Filled", filled);
                Effect.SetValue("EnableZ", false);
                Effect.SetValue("antiAlias", AntiAlias);

                Effect.EndPass();

                Render.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }

            Effect.End();

            Render.Device.VertexDeclaration = declaration;
        }

        #endregion
    }
}