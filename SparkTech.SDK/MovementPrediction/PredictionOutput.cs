//  -------------------------------------------------------------------
// 
//  Last updated: 21/08/2017
//  Created: 29/07/2017
// 
//  Copyright (c) Entropy, 2017 - 2017
// 
//  PredictionOutput.cs is a part of SparkTech
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
    using System;
    using System.Collections.Generic;

    using SharpDX;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.League;

    public class PredictionOutput
    {
        internal int _aoeTargetsHitCount;
        internal List<IHero> _aoeTargetsHit = new List<IHero>();
        internal Vector3 _castPosition = Vector3.Zero;
        internal Vector3 _unitPosition = Vector3.Zero;
        public List<IUnit> CollisionObjects = new List<IUnit>();
        public HitChance Hitchance = HitChance.None;
        public PredictionInput Input;

        public List<IHero> AoeTargetsHit
        {
            get => this._aoeTargetsHit;
            set => this._aoeTargetsHit = value;
        }

        public int AoeTargetsHitCount
        {
            get => Math.Max(this._aoeTargetsHitCount, this.AoeTargetsHit.Count);
            set => this._aoeTargetsHitCount = value;
        }

        public Vector3 CastPosition
        {
            get
            {
                if (this._castPosition.IsValid() && this._castPosition.ToVector2().IsValid())
                    return this._castPosition;
                return this.Input.Unit != null && this.Input.Unit.IsValid ? this.Input.Unit.PreviousPosition : Vector3.Zero;
            }
            set => this._castPosition = value;
        }

        public Vector3 UnitPosition
        {
            get
            {
                if (this._unitPosition.IsValid() && this._unitPosition.ToVector2().IsValid())
                    return this._unitPosition;
                return this.Input.Unit != null && this.Input.Unit.IsValid ? this.Input.Unit.PreviousPosition : Vector3.Zero;
            }
            set => this._unitPosition = value;
        }
    }
}