//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 26/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  Sector.cs is a part of SparkTech
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
    ///     Class Sector
    /// </summary>
    /// <seealso cref="LinearPolygon" />
    public sealed class Sector : LinearPolygon
    {
        #region Fields

        private Vector3 _center;

        private Vector3 _direction;

        private uint _quality;

        private float _radius;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Sector" /> class.
        /// </summary>
        /// <param name="worldCenter">The world center.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="quality">The quality.</param>
        public Sector(Vector3 worldCenter, Vector3 direction, float angle, float radius, uint quality)
        {
            this._center = worldCenter;
            this.Angle = angle;
            this._radius = radius;
            this._quality = quality;
            this._direction = direction;

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
        public float Angle { get; set; }

        /// <summary>
        ///     Gets or sets the center point.
        /// </summary>
        /// <value>
        ///     The center point.
        /// </value>
        public Vector3 Center
        {
            get => this._center;

            set
            {
                this._center = value;
                this.Update();
            }
        }

        /// <summary>
        ///     Gets or sets the direction.
        /// </summary>
        /// <value>
        ///     The direction.
        /// </value>
        public Vector3 Direction
        {
            get => this._direction;

            set
            {
                this._direction = value;
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

        #endregion

        #region Methods

        protected override void Update()
        {
            // Number of points on sector.
            var pointCount = this.Quality + 2;

            // Create vector to hold points + add center as a point.
            this.WorldPoints = new Vector3[pointCount + 1];
            this.WorldPoints[0] = this.Center;

            // Convert normalised direction vector to angle.
            // This will be the angle of the first point on the circle.
            var angle = (float)Math.Atan2(this.Direction.Z, this.Direction.X);

            // For each point calculate its position and add it to the points vector.
            for (var i = 0; i < pointCount; i++)
            {
                // Calculate the x, y, z pos.
                var x = this.Radius * (float)Math.Cos(angle) + this.Center.X;
                var z = this.Radius * (float)Math.Sin(angle) + this.Center.Z;
                var y = this.Center.Y;

                // Add the point to the vector.
                this.WorldPoints[i + 1] = new Vector3(x, y, z);

                // Increment the angle based on the arc angle.
                angle += this.Angle.ToRadians() / pointCount;
            }
        }

        #endregion
    }
}