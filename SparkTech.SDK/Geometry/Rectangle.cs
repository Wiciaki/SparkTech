//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 26/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  Rectangle.cs is a part of SparkTech
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

    using SharpDX;

    using SparkTech.SDK.Util.Vector;

    #endregion

    /// <summary>
    ///     Class Rectangle
    /// </summary>
    /// <seealso cref="LinearPolygon" />
    public sealed class Rectangle : LinearPolygon
    {
        #region Fields

        private Vector3 _endPoint;

        private Vector3 _startPoint;

        private float _width;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Rectangle" /> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="width">The width.</param>
        public Rectangle(Vector3 start, Vector3 end, float width)
        {
            this._startPoint = start;
            this._endPoint = end;
            this._width = width;

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
        public Vector3 Direction => (this.EndPoint - this.StartPoint).Normalized();

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

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public float Width
        {
            get => this._width;

            set
            {
                this._width = value;
                this.Update();
            }
        }

        #endregion

        #region Methods

        protected override void Update()
        {
            this.WorldPoints = new[]
                               {
                                   this.StartPoint + this.Width * this.Direction.PerpendicularClockwise(),
                                   this.StartPoint - this.Width * this.Direction.PerpendicularClockwise(),
                                   this.EndPoint - this.Width * this.Direction.PerpendicularClockwise(),
                                   this.EndPoint + this.Width * this.Direction.PerpendicularClockwise()
                               };
        }

        #endregion
    }
}