//  -------------------------------------------------------------------
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

namespace SparkTech.SDK.Geometry
{
    #region Using Directives

    using System;

    using SharpDX;

    using SparkTech.SDK.Util.Vector;

    #endregion

    public sealed class Circle : IPolygon
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Circle" /> class.
        /// </summary>
        /// <param name="worldCenter">The world center.</param>
        /// <param name="radius">The radius.</param>
        public Circle(Vector3 worldCenter, float radius)
        {
            this.Center = worldCenter;
            this.Radius = radius;
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
        ///     Gets or sets the radius.
        /// </summary>
        /// <value>
        ///     The radius.
        /// </value>
        public float Radius { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Finds the intersection of two circles.
        /// </summary>
        /// <param name="circle">The other circle.</param>
        /// <returns>
        ///     <see>
        ///         <cref>Vector2[]</cref>
        ///     </see>
        ///     of the intersection points.</returns>
        public Vector2[] FindIntersection(Circle circle)
        {
            var Distance = this.Center.Distance(circle.Center);
            if (Distance > this.Radius + circle.Radius || Distance <= Math.Abs(this.Radius - circle.Radius))
            {
                return new Vector2[0];
            }

            var A = (this.Radius * this.Radius - circle.Radius * circle.Radius + Distance * Distance) / (2 * Distance);
            var H = (float)Math.Sqrt(this.Radius * this.Radius - A * A);
            var Direction = (circle.Center - this.Center).Normalized();
            var PA = this.Center + A * Direction;
            var Loc1 = PA + H * Direction.PerpendicularClockwise();
            var Loc2 = PA - H * Direction.PerpendicularClockwise();
            return new[]
                   {
                       Loc1.To2D(), Loc2.To2D()
                   };
        }

        /// <summary>
        ///     Finds the intersection of a circle and a line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>
        ///     <see>
        ///         <cref>Vector2[]</cref>
        ///     </see>
        ///     of the intersection points.</returns>
        public Vector2[] FindIntersection(Line line)
        {
            return line.FindIntersection(this);
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
            return worldPoint.Distance(this.Center) <= this.Radius;
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
        ///     Renders the circle in the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        public void Render(Color color, float thickness = 1f)
        {
            Rendering.Circle.Render(color, this.Radius, thickness, false, 0.65f, this.Center);
        }

        #endregion
    }
}