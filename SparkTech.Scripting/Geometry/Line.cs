//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 26/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  Line.cs is a part of SparkTech
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

    using SharpDX;

    #endregion

    /// <summary>
    ///     Class Line
    /// </summary>
    /// <seealso cref="SparkTech.Geometry.LinearPolygon" />
    public sealed class Line : LinearPolygon
    {
        #region Fields

        private Vector3 _rootPoint1;

        private Vector3 _rootPoint2;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> class.
        /// </summary>
        /// <param name="rootPoint1">The root point1.</param>
        /// <param name="rootPoint2">The root point2.</param>
        public Line(Vector3 rootPoint1, Vector3 rootPoint2)
        {
            this._rootPoint1 = rootPoint1;
            this._rootPoint2 = rootPoint2;

            this.Update();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the first root point.
        /// </summary>
        /// <value>
        ///     The first root point.
        /// </value>
        public Vector3 RootPoint1
        {
            get => this._rootPoint1;

            set
            {
                this._rootPoint1 = value;
                this.Update();
            }
        }

        /// <summary>
        ///     Gets or sets the second root point.
        /// </summary>
        /// <value>
        ///     The second root point.
        /// </value>
        public Vector3 RootPoint2
        {
            get => this._rootPoint2;

            set
            {
                this._rootPoint2 = value;
                this.Update();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Finds the intersection of two lines.
        /// </summary>
        /// <param name="line1">The other line.</param>
        /// <returns>The <see cref="Vector2" /> intersection point, or <see cref="Vector2.Zero" /> if the lines are parallel.</returns>
        public Vector2 FindIntersection(Line line1)
        {
            var A1 = this.RootPoint1.Y - line1.RootPoint1.Y;
            var B1 = line1.RootPoint1.X - this.RootPoint1.X;
            var C1 = A1 * line1.RootPoint1.X + B1 * line1.RootPoint1.Y;

            var A2 = this.RootPoint2.Y - line1.RootPoint2.Y;
            var B2 = line1.RootPoint2.X - this.RootPoint2.X;
            var C2 = A2 * line1.RootPoint2.X + B2 * line1.RootPoint2.Y;

            var delta = A1 * B2 - A2 * B1;
            return delta == 0 ? Vector2.Zero : new Vector2((B2 * C1 - B1 * C2) / delta, (A1 * C2 - A2 * C1) / delta);
        }

        /// <summary>
        ///     Finds the intersection of the line and a circle.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <returns><see cref="Vector2[]" /> of all the intersections found.</returns>
        public Vector2[] FindIntersection(Circle circle)
        {
            float t;

            var dx = this.RootPoint2.X - this.RootPoint1.X;
            var dy = this.RootPoint2.Y - this.RootPoint1.Y;

            var a = dx * dx + dy * dy;
            var b = 2 * (dx * (this.RootPoint1.X - circle.Center.X) + dy * (this.RootPoint1.Y - circle.Center.Y));
            var c = (this.RootPoint1.X - circle.Center.X) * (this.RootPoint1.X - circle.Center.X)
                    + (this.RootPoint1.Y - circle.Center.Y) * (this.RootPoint1.Y - circle.Center.Y)
                    - circle.Radius * circle.Radius;
            var det = b * b - 4 * a * c;

            if (Math.Abs(a) <= float.Epsilon || det < 0)
            {
                return new Vector2[] { }; 
            }

            if (det == 0)
            {
                t = -b / (2 * a);
                return new[]
                       {
                           new Vector2(this.RootPoint1.X + t * dx, this.RootPoint1.Y + t * dy)
                       };
            }

            t = (float)((-b + Math.Sqrt(det)) / (2 * a));
            var int1 = new Vector2(this.RootPoint1.X + t * dx, this.RootPoint1.Y + t * dy);
            t = (float)((-b - Math.Sqrt(det)) / (2 * a));

            return new[]
                   {
                       int1, new Vector2(this.RootPoint1.X + t * dx, this.RootPoint1.Y + t * dy)
                   };
        }

        /// <summary>
        ///     Renders the line with the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        public override void Render(Color color, float thickness = 1)
        {
            Rendering.Line.Render(color, thickness, true, this.WorldPoints);
        }

        #endregion

        #region Methods

        protected override void Update()
        {
            this.WorldPoints = new[]
                               {
                                   this.RootPoint1, this.RootPoint2
                               };
        }

        #endregion
    }
}