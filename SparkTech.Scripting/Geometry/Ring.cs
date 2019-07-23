//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 26/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  Ring.cs is a part of SparkTech
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

namespace SparkTech.Geometry
{
    #region Using Directives

    using System;

    using SparkTech.Utils;

    using SharpDX;

    #endregion

    /// <summary>
    ///     Class Ring
    /// </summary>
    /// <seealso cref="SparkTech.Geometry.IPolygon" />
    public sealed class Ring : IPolygon
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ring" /> class.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="outerRadius">The outer radius.</param>
        /// <param name="innerRadius">The inner radius.</param>
        public Ring(Vector3 center, float outerRadius, float innerRadius)
        {
            this.Center = center;
            this.OuterRadius = outerRadius;
            this.InnerRadius = innerRadius;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the center point.
        /// </summary>
        /// <value>
        ///     The center point.
        /// </value>
        public Vector3 Center { get; set; }

        /// <summary>
        ///     Gets or sets the inner radius.
        /// </summary>
        /// <value>
        ///     The inner radius.
        /// </value>
        public float InnerRadius { get; set; }

        /// <summary>
        ///     Gets or sets the outer radius.
        /// </summary>
        /// <value>
        ///     The outer radius.
        /// </value>
        public float OuterRadius { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Determines whether [is inside polygon] [the specified world point].
        /// </summary>
        /// <param name="worldPoint">The world point.</param>
        /// <returns>
        ///     <c>true</c> if [is inside polygon] [the specified world point]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInsidePolygon(Vector2 worldPoint)
        {
            return this.IsInsidePolygon(worldPoint.ToFlat3D());
        }

        /// <summary>
        ///     Determines whether [is inside polygon] [the specified world point].
        /// </summary>
        /// <param name="worldPoint">The world point.</param>
        /// <returns>
        ///     <c>true</c> if [is inside polygon] [the specified world point]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInsidePolygon(Vector3 worldPoint)
        {
            var distance = Vector3.Distance(worldPoint, this.Center);
            return distance <= this.OuterRadius && distance >= this.InnerRadius;
        }

        /// <summary>
        ///     Determines whether [is outside polygon] [the specified world point].
        /// </summary>
        /// <param name="worldPoint">The world point.</param>
        /// <returns>
        ///     <c>true</c> if [is outside polygon] [the specified world point]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOutsidePolygon(Vector2 worldPoint)
        {
            return this.IsOutsidePolygon(worldPoint.ToFlat3D());
        }

        /// <summary>
        ///     Determines whether [is outside polygon] [the specified world point].
        /// </summary>
        /// <param name="worldPoint">The world point.</param>
        /// <returns>
        ///     <c>true</c> if [is outside polygon] [the specified world point]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOutsidePolygon(Vector3 worldPoint)
        {
            return !this.IsInsidePolygon(worldPoint);
        }

        /// <summary>
        ///     Renders the ring in the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        public void Render(Color color, float thickness = 1f)
        {
            Rendering.Circle.Render(color, this.OuterRadius, thickness, false, 0.65f, this.Center);
            Rendering.Circle.Render(color, this.InnerRadius, thickness, false, 0.65f, this.Center);
        }

        #endregion
    }
}