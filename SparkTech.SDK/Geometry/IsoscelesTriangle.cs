//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 26/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  IsoscelesTriangle.cs is a part of SparkTech
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
/*
namespace SparkTech.SDK.Geometry
{
    #region Using Directives

    using SharpDX;

    using SparkTech.SDK.Util.Vector;

    #endregion

    /// <summary>
    ///     Class IsoscelesTriangle
    /// </summary>
    /// <seealso cref="Triangle" />
    public sealed class IsoscelesTriangle : Triangle
    {
        #region Fields

        private float _height;

        private bool _up;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IsoscelesTriangle" /> class.
        /// </summary>
        /// <param name="worldRoot1">The first world root.</param>
        /// <param name="worldRoot2">The second world root.</param>
        /// <param name="height">The height.</param>
        /// <param name="up">if set to <c>true</c> [facing up].</param>
        public IsoscelesTriangle(Vector3 worldRoot1, Vector3 worldRoot2, float height, bool up)
        {
            this._rootPoint1 = worldRoot1;
            this._rootPoint2 = worldRoot2;
            this._height = height;
            this._up = up;

            this.Update();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the direction.
        /// </summary>
        /// <value>
        ///     The direction.
        /// </value>
        public Vector3 Direction => (this.RootPoint2 - this.RootPoint1).Normalized();

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public float Height
        {
            get => this._height;

            set
            {
                this._height = value;
                this.Update();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IsoscelesTriangle" /> is facing up.
        /// </summary>
        /// <value>
        ///     <c>true</c> if facing up; otherwise, <c>false</c>.
        /// </value>
        public bool Up
        {
            get => this._up;

            set
            {
                this._up = value;
                this.Update();
            }
        }

        #endregion

        #region Methods

        protected override void Update()
        {
            var midpoint = this.RootPoint1.Midpoint(this.RootPoint2);
            this._rootPoint3 = midpoint + this.Height
                               * (this.Up
                                      ? this.Direction.PerpendicularClockwise()
                                      : this.Direction.PerpendicularAntiClockwise());

            base.Update();
        }

        #endregion
    }
}*/