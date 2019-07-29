//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 26/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  LinearPolygon.cs is a part of SparkTech
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
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;
    using SparkTech.SDK.Clipper;
    using SparkTech.SDK.Util.Vector;

    #endregion

    public abstract class LinearPolygon : IPolygon
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearPolygon" /> class.
        /// </summary>
        /// <param name="worldPoints">The world points.</param>
        protected LinearPolygon(params Vector3[] worldPoints)
        {
            this.WorldPoints = worldPoints;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the world points.
        /// </summary>
        /// <value>
        ///     The world points.
        /// </value>
        public Vector3[] WorldPoints { get; set; }

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
            return !this.IsInsidePolygon(worldPoint.ToFlat3D());
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
            return !this.IsOutsidePolygon(worldPoint);
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
            var point = new IntPoint(worldPoint.X, worldPoint.Z);
            return Clipper.PointInPolygon(point, this.ToClipperPath()) != 1;
        }

        /// <summary>
        ///     Renders the linear polygon with the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        public virtual void Render(Color color, float thickness = 1f)
        {
            Rendering.Line.Render(color, thickness, true, this.WorldPoints);
            Rendering.Line.Render(
                color,
                thickness,
                true,
                this.WorldPoints[0],
                this.WorldPoints[^1]);
        }

        /// <summary>
        ///     Converts the polygon to a clipper path, which is a list of <see cref="IntPoint" />
        /// </summary>
        /// <returns><see cref="List{IntPoint}" /> of the world-space points</returns>
        public List<IntPoint> ToClipperPath()
        {
            var result = new List<IntPoint>(this.WorldPoints.Length);
            result.AddRange(this.WorldPoints.Select(p => new IntPoint(p.X, p.Z)));
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Updates and calculates this polygon's <see cref="WorldPoints" /> using its simpler to use custom implementation.
        /// </summary>
        protected abstract void Update();

        #endregion
    }
}