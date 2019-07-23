//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 26/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  Triangle.cs is a part of SparkTech
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

    using SharpDX;

    #endregion

    /// <summary>
    ///     Class Triangle
    /// </summary>
    /// <seealso cref="SparkTech.Geometry.LinearPolygon" />
    public class Triangle : LinearPolygon
    {
        #region Fields

        protected Vector3 _rootPoint1;

        protected Vector3 _rootPoint2;

        protected Vector3 _rootPoint3;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Triangle" /> class.
        /// </summary>
        /// <param name="worldRoot1">The first world-space root point.</param>
        /// <param name="worldRoot2">The second world-space root point.</param>
        /// <param name="worldRoot3">The third world-space root point.</param>
        public Triangle(Vector3 worldRoot1, Vector3 worldRoot2, Vector3 worldRoot3)
        {
            this._rootPoint1 = worldRoot1;
            this._rootPoint2 = worldRoot2;
            this._rootPoint3 = worldRoot3;

            this.Update();
        }

        protected Triangle()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the root point.
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

        /// <summary>
        ///     Gets or sets the third root point.
        /// </summary>
        /// <value>
        ///     The third root point.
        /// </value>
        public Vector3 RootPoint3
        {
            get => this._rootPoint3;

            set
            {
                this._rootPoint3 = value;
                this.Update();
            }
        }

        #endregion

        #region Methods

        protected override void Update()
        {
            this.WorldPoints = new[]
                               {
                                   this.RootPoint1, this.RootPoint2, this.RootPoint3
                               };
        }

        #endregion
    }
}