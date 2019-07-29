//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 26/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  Arc.cs is a part of SparkTech
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

    /// <summary>
    ///     Class Arc
    /// </summary>
    /// <seealso cref="LinearPolygon" />
    public sealed class Arc : LinearPolygon
    {
        #region Fields

        private float _angle;

        private Vector3 _endPoint;

        private uint _quality;

        private float _radius;

        private Vector3 _startPoint;

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SparkTech.SDK.Geometry.Arc" /> class.
        /// </summary>
        /// <param name="worldStart">The start point in world-space.</param>
        /// <param name="worldEnd">The end point in world-space.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="quality">The quality.</param>
        public Arc(Vector3 worldStart, Vector3 worldEnd, float radius, float angle, uint quality)
        {
            this._startPoint = worldStart;
            this._endPoint = worldEnd;
            this._radius = radius;
            this._angle = angle;
            this._quality = quality;

            this.Update();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the angle.
        /// </summary>
        /// <value>
        ///     The angle.
        /// </value>
        public float Angle
        {
            get => this._angle;

            set
            {
                this._angle = value;
                this.Update();
            }
        }

        /// <summary>
        ///     Gets or sets the end point.
        /// </summary>
        /// <value>
        ///     The end point.
        /// </value>
        public Vector3 EndPoint
        {
            get => this._endPoint;

            set
            {
                this._endPoint = value;
                this.Update();
            }
        }

        /// <summary>
        ///     Gets or sets the quality.
        /// </summary>
        /// <value>
        ///     The quality.
        /// </value>
        public uint Quality
        {
            get => this._quality;

            set
            {
                this._quality = value;
                this.Update();
            }
        }

        /// <summary>
        ///     Gets or sets the radius.
        /// </summary>
        /// <value>
        ///     The radius.
        /// </value>
        public float Radius
        {
            get => this._radius;

            set
            {
                this._radius = value;
                this.Update();
            }
        }

        /// <summary>
        ///     Gets or sets the start point.
        /// </summary>
        /// <value>
        ///     The start point.
        /// </value>
        public Vector3 StartPoint
        {
            get => this._startPoint;

            set
            {
                this._startPoint = value;
                this.Update();
            }
        }

        #endregion

        #region Methods

        protected override void Update()
        {
            var midpoint = new Vector3(
                (this._startPoint.X + this._endPoint.X) / 2,
                this._startPoint.Y,
                (this._startPoint.Z + this._endPoint.Z) / 2);
            var direction = (this._endPoint - this._startPoint).Normalized();
            var perpendicular = new Vector3(direction.Z, direction.Y, -direction.X);

            var distance = (float)Math.Sqrt(
                (midpoint.X - this._startPoint.X) * (midpoint.X - this._startPoint.X)
                + (midpoint.Z - this._startPoint.Z) * (midpoint.Z - this._startPoint.Z));
            var radius = distance / (float)Math.Sin(0.5 * this._angle);
            var length = distance / (float)Math.Tan(0.5 * this._angle);

            var centre = midpoint + perpendicular * length;

            var pointCount = this._quality + 2;
            this.WorldPoints = new Vector3[pointCount + 1];
            this.WorldPoints[0] = centre;

            var angleDir = (this._startPoint - centre).Normalized();
            var angle = (float)Math.Atan2(angleDir.Z, angleDir.X);

            for (var i = 0; i < pointCount; i++)
            {
                var x = radius * (float)Math.Cos(angle) + centre.X;
                var z = radius * (float)Math.Sin(angle) + centre.Z;
                var y = centre.Y;

                this.WorldPoints[i + 1] = new Vector3(x, y, z);

                angle += this._angle.ToRadians() / pointCount;
            }
        }

        #endregion
    }
}