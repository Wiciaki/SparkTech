//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 29/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  PredictionInput.cs is a part of SparkTech
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

namespace SparkTech.SDK.MovementPrediction
{
    public class PredictionInput
    {
        /*#region Fields

        private Vector3 from;

        #endregion

        #region Public Properties

        // SpellInfo

        /// <summary>
        ///     How many targets the spell can hit
        /// </summary>
        public int CollisionCount { get; set; }

        public CollisionableObjects CollisionObjects { get; set; } =
            CollisionableObjects.Minions | CollisionableObjects.YasuoWall;

        public float Delay { get; set; }

        public Vector3 From
        {
            get => this.from.IsZero ? LocalPlayer.Instance.Position : this.from;

            set => this.from = value;
        }

        public AIBaseClient FromUnit { get; set; } = LocalPlayer.Instance;

        public bool HasCollision { get; set; }

        /// <summary>
        ///     If the spell is area of effect
        /// </summary>
        public bool IsAOE { get; set; }

        public float Radius { get; set; } = 1f;

        public float Range { get; set; } = float.MaxValue;

        public float RealRadius => this.UseBoundingRadius ? this.Radius + this.FromUnit.BoundingRadius : this.Radius;

        ///public Spell SelectedSpell { get; set; }

        public float Speed { get; set; } = float.MaxValue;

        /// <summary>
        ///     List of targets to get the prediction
        /// </summary>
        public AIBaseClient Target { get; set; }

        public bool UseBoundingRadius { get; set; } = true;

        #endregion

        //private Vector3 rangeCheckFrom;*/
    }
}