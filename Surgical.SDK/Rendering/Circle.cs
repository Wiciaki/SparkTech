﻿//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 26/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  Circle.cs is a part of SparkTech
// 
//  SparkTech is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  SparkTech is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  You should have received a copy of the GNU General Public License
//  along with SparkTech. If not, see <http://www.gnu.org/licenses/>.
// 
//  -------------------------------------------------------------------

namespace Surgical.SDK.Rendering
{
    using SharpDX;
    using SharpDX.Direct3D9;

    using Surgical.SDK.Properties;

    public static class Circle
    {
        #region Constants

        /// <summary>
        ///     The maximum radius a circle can be drawn with
        /// </summary>
        private const float MaxRadius = 20000f;

        private const float AntiAlias = 0.65f;

        private static readonly Effect Effect;

        private static readonly EffectHandle EffectHandle;

        private static readonly VertexBuffer VertexBuffer;

        private static readonly VertexDeclaration VertexDeclaration;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="Circle" /> class.
        /// </summary>
        static Circle()
        {
            // Initialize the vertex buffer, specifying its size, usage, format and pool
            VertexBuffer = new VertexBuffer(Render.Device, Utilities.SizeOf<Vector4>() * 3, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

            // Lock and write the vertices onto the vertex buffer
            VertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(
                new[] { new Vector4(-MaxRadius, 0f, -MaxRadius, 1.0f), new Vector4(0f, 0f, MaxRadius, 1.0f), new Vector4(MaxRadius, 0f, -MaxRadius, 1.0f) });

            VertexBuffer.Unlock();

            // Specify the vertex elements to be used by the shader
            var vertexElements = new[] { new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0), VertexElement.VertexDeclarationEnd };

            // Initialize the vertex decleration using the previously created vertex elements
            VertexDeclaration = new VertexDeclaration(Render.Device, vertexElements);

            #endregion

            // Load the effect from memory
            Effect = Effect.FromMemory(Render.Device, Resources.RenderEffectCompiled, ShaderFlags.None);

            // Set the only technique in the shaders
            EffectHandle = Effect.GetTechnique(0);

            Render.OnLostDevice += () => Effect.OnLostDevice();
            Render.OnResetDevice += () => Effect.OnResetDevice();

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
            // Save the current VertexDecleration for restoring later
            var declaration = Render.Device.VertexDeclaration;

            // Set the current effect technique and begin the shading process
            Effect.Technique = EffectHandle;
            Effect.Begin();

            // Send data to the GPU using the Direct3DDevice
            Render.Device.SetStreamSource(0, VertexBuffer, 0, Utilities.SizeOf<Vector4>());
            Render.Device.VertexDeclaration = VertexDeclaration;

            var multiplier = Render.View * Render.Projection;

            // Loop through the world-space positions to draw the circle
            foreach (var position in worldPositions)
            {
                // Send all the global variables to the shader
                Effect.BeginPass(0);

                Effect.SetValue("ProjectionMatrix", Matrix.Translation(position) * multiplier);
                Effect.SetValue("Color", new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
                Effect.SetValue("Radius", radius);
                Effect.SetValue("Width", thickness);
                Effect.SetValue("Filled", filled);
                Effect.SetValue("EnableZ", false);
                Effect.SetValue("antiAlias", AntiAlias);

                Effect.EndPass();

                // Draw the primitives in the shader
                Render.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }

            // End the shading process
            Effect.End();

            // Restore the previous VertexDecleration
            Render.Device.VertexDeclaration = declaration;
        }

        #endregion
    }
}