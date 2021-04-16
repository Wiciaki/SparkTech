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
    using SharpDX;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.League;

    public class PredictionInput
    {
        internal Vector3 _from;
        internal Vector3 _rangeCheckFrom;
        public bool AddHitBox = true;
        public bool Aoe;
        public bool Collision;
        public CollisionableObjects[] CollisionObjects = new CollisionableObjects[]
        {
            CollisionableObjects.Minions,
            CollisionableObjects.YasuoWall
        };
        public float Delay;
        public float Radius = 1f;
        public float Range = float.MaxValue;
        public float Speed = float.MaxValue;
        public SpellType Type = SpellType.None;
        public IUnit Unit = ObjectManager.Player;

        public Vector3 From
        {
            get => !this._from.ToVector2().IsValid() ? ObjectManager.Player.PreviousPosition : this._from;
            set => this._from = value;
        }

        public Vector3 RangeCheckFrom
        {
            get
            {
                if (this._rangeCheckFrom.ToVector2().IsValid())
                    return this._rangeCheckFrom;
                return !this.From.ToVector2().IsValid() ? ObjectManager.Player.PreviousPosition : this.From;
            }
            set => this._rangeCheckFrom = value;
        }

        public float RealRadius => !this.AddHitBox ? this.Radius : this.Radius + this.Unit.BoundingRadius;
    }
}