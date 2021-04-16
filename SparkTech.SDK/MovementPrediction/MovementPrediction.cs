namespace SparkTech.SDK.MovementPrediction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.Detectors;
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.League;

    public class MovementPrediction : IMovementPrediction
    {
        public Menu Menu { get; } = new Menu("Ported E#") { new MenuText("a") };

        public JObject GetTranslations()
        {
            return null;
        }

        public void Start()
        {

        }

        public void Pause()
        {

        }

        public PredictionOutput Predict(PredictionInput predInput) => this.GetPrediction(predInput, true, true);

        public PredictionOutput GetPrediction(
          PredictionInput input,
          bool ft,
          bool checkCollision)
        {
            PredictionOutput predictionOutput = null;
        //    if (input == null)
        //        throw new Exception("PredictionInput cant be null!");
        //    if (input.Unit == null || !input.Unit.IsValidTarget(false))
        //        return new PredictionOutput() { Input = input };
        //    if (input.Unit.Type == GameObjectType.AIHeroClient && input.Unit.CharName == "Yuumi" && ObjectManager.Get<IHero>().Any(x => x.IsValidTarget(false) && x.Team == input.Unit.Team && x.HasBuff("YuumiWAlly") && x.Distance(input.Unit) <= 50.0))
        //        return new PredictionOutput() { Input = input };
        //    if (ft)
        //    {
        //        input.Delay += Game.Ping / 2000f + 0.06f;
        //        if (input.Aoe)
        //            return AoEPrediction.GetPrediction(input);
        //    }
        //    if (Math.Abs(input.Range - float.MaxValue) > 1.40129846432482E-45 && Vector3.DistanceSquared(input.Unit.Position, input.RangeCheckFrom) > Math.Pow(input.Range * 1.5, 2.0))
        //        return new PredictionOutput() { Input = input };
        //    if (input.Unit.Type == GameObjectType.AIHeroClient)
        //    {
        //        if (input.Unit.IsDashing())
        //        {
        //            predictionOutput = GetDashingPrediction(input);
        //        }
        //        else
        //        {
        //            double remainingImmobileT = UnitIsImmobileUntil(input.Unit);
        //            if (remainingImmobileT >= 0.0)
        //                predictionOutput = GetImmobilePrediction(input, remainingImmobileT);
        //        }
        //    }
        //    if (predictionOutput == null)
        //        predictionOutput = GetPositionOnPath(input, input.Unit.GetWaypoints(), input.Unit.MoveSpeed, true);
        //    if ((double)Math.Abs(input.Range - float.MaxValue) > 1.40129846432482E-45)
        //    {
        //        if (predictionOutput.Hitchance >= HitChance.High && Vector3.DistanceSquared(input.RangeCheckFrom, input.Unit.Position) > Math.Pow(input.Range + input.RealRadius * 3.0 / 4.0, 2.0))
        //            predictionOutput.Hitchance = HitChance.Medium;
        //        if (Vector3.DistanceSquared(input.RangeCheckFrom, predictionOutput.UnitPosition) > Math.Pow(input.Range + (input.Type == SpellType.Circle ? input.RealRadius : 0.0), 2.0))
        //            predictionOutput.Hitchance = HitChance.OutOfRange;
        //    }
        //    if (checkCollision && input.Collision && predictionOutput.Hitchance > HitChance.None)
        //    {
        //        List<IUnit> collision = Collisions.GetCollision(new List<Vector3>()
        //{
        //  predictionOutput.CastPosition
        //}, input);
        //        if (collision.Count > 0)
        //        {
        //            collision.RemoveAll(x => x == null || !x.IsValid || x.Compare(input.Unit));
        //            predictionOutput.CollisionObjects = collision;
        //            predictionOutput.Hitchance = HitChance.Collision;
        //            return predictionOutput;
        //        }
        //    }
            return predictionOutput;
        }

        private static PredictionOutput GetDashingPrediction(PredictionInput input)
        {
            //var gapcloserInfo = input.Unit.GetGapcloserInfo();
            PredictionOutput predictionOutput = new PredictionOutput()
            {
                Input = input
            };
            //if (gapcloserInfo != null && gapcloserInfo.SpellName != "NullDash")
            //{
            //    Vector2 vector2 = gapcloserInfo.EndPosition.ToVector2();
            //    PredictionInput input1 = input;
            //    List<Vector2> path = new List<Vector2>();
            //    path.Add(input.Unit.Position.ToVector2());
            //    path.Add(vector2);
            //    double speed = (double)gapcloserInfo.Speed;
            //    PredictionOutput positionOnPath = GetPositionOnPath(input1, path, (float)speed);
            //    if (positionOnPath.Hitchance >= HitChance.High && (double)positionOnPath.UnitPosition.ToVector2().Distance(input.Unit.Position.ToVector2(), vector2, true) < Math.Sqrt(200.0))
            //    {
            //        positionOnPath.CastPosition = positionOnPath.UnitPosition;
            //        positionOnPath.Hitchance = HitChance.Dash;
            //        return positionOnPath;
            //    }
            //    if (gapcloserInfo.StartPosition.Distance(gapcloserInfo.EndPosition) > 200.0 && input.Delay / 2.0 + input.From.ToVector2().Distance(vector2) / input.Speed - 0.25 <= input.Unit.Distance(vector2) / (double)gapcloserInfo.Speed + input.RealRadius / input.Unit.MoveSpeed)
            //        return new PredictionOutput()
            //        {
            //            CastPosition = vector2.ToVector3(),
            //            UnitPosition = vector2.ToVector3(),
            //            Hitchance = HitChance.Dash
            //        };
            //    predictionOutput.CastPosition = gapcloserInfo.EndPosition;
            //    predictionOutput.UnitPosition = gapcloserInfo.EndPosition;
            //}
            //else
            //{
                input.Delay += 0.1f;
                var dashInfo = input.Unit.GetDash();
                Vector2 vector2 = dashInfo.Path.Last().ToVector2();
                PredictionInput input1 = input;
                List<Vector2> path = new List<Vector2>();
                path.Add(dashInfo.StartPosition.ToVector2());
                path.Add(vector2);
                float speed = dashInfo.Speed;
                PredictionOutput positionOnPath = GetPositionOnPath(input1, path, speed);
                if (positionOnPath.Hitchance >= HitChance.High && positionOnPath.UnitPosition.ToVector2().Distance(input.Unit.Position.ToVector2(), vector2, true) < 200.0)
                {
                    positionOnPath.CastPosition = positionOnPath.UnitPosition;
                    positionOnPath.Hitchance = HitChance.Dash;
                    return positionOnPath;
                }
                if (dashInfo.StartPosition.ToVector2().Distance(dashInfo.EndPosition.ToVector2()) > 200.0 && input.Delay / 2.0 + input.From.ToVector2().Distance(vector2) / input.Speed - 0.25 <= input.Unit.Position.ToVector2().Distance(vector2) / dashInfo.Speed + input.RealRadius / input.Unit.MoveSpeed)
                    return new PredictionOutput()
                    {
                        CastPosition = vector2.ToVector3(),
                        UnitPosition = vector2.ToVector3(),
                        Hitchance = HitChance.Dash
                    };
                predictionOutput.CastPosition = vector2.ToVector3();
                predictionOutput.UnitPosition = vector2.ToVector3();
            //}
            return predictionOutput;
        }

        private static PredictionOutput GetImmobilePrediction(
          PredictionInput input,
          double remainingImmobileT)
        {
            Vector3 position = input.Unit.Position;
            float num = input.Delay + input.Unit.Distance(input.From) / input.Speed;
            if ((double)num <= remainingImmobileT + (double)input.RealRadius / (double)input.Unit.MoveSpeed)
                return new PredictionOutput()
                {
                    CastPosition = position,
                    UnitPosition = position,
                    Hitchance = HitChance.Immobile
                };
            if ((double)num - 0.200000002980232 <= remainingImmobileT + (double)input.RealRadius / (double)input.Unit.MoveSpeed)
                return new PredictionOutput()
                {
                    CastPosition = position,
                    UnitPosition = position,
                    Hitchance = HitChance.VeryHigh
                };
            return new PredictionOutput()
            {
                Input = input,
                CastPosition = position,
                UnitPosition = position,
                Hitchance = HitChance.Medium
            };
        }

        private static double UnitIsImmobileUntil(IUnit unit)
        {
            return unit.Buffs.Where(buff =>
{
    if (!buff.IsValid || !buff.IsActive || Game.Time > buff.EndTime)
        return false;
    return buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || (buff.Type == BuffType.Stun || buff.Type == BuffType.Suppression) || (buff.Type == BuffType.Snare || buff.Type == BuffType.Fear || (buff.Type == BuffType.Taunt || buff.Type == BuffType.Knockback)) || buff.Type == BuffType.Asleep;
}).Aggregate(0f, (current, buff) => Math.Max(current, buff.EndTime)) - Game.Time;
        }

        private static PredictionOutput GetPositionOnPath(
          PredictionInput input,
          List<Vector2> path,
          float speed = -1f,
          bool needToFixSpeed = false)  
        {
            if (needToFixSpeed && (double)input.Unit.Distance(input.From) < 250.0)
                speed *= 1.5f;
            speed = (double)Math.Abs(speed - -1f) < 1.40129846432482E-45 ? input.Unit.MoveSpeed : speed;
            Vector3 previousPosition = input.Unit.PreviousPosition;
            if (path.Count <= 1 || input.Unit.IsWindingUp && !input.Unit.IsDashing())
                return new PredictionOutput()
                {
                    Input = input,
                    UnitPosition = previousPosition,
                    CastPosition = previousPosition,
                    Hitchance = HitChance.VeryHigh
                };
            float pathLength = path.PathLength();
            if (path.Count == 2 && (double)pathLength < 5.0 && (input.Unit.CharName == "PracticeTool_TargetDummy" || input.Unit is IMinion minion && (minion.IsMinion() || minion.IsJungle())))
                return new PredictionOutput()
                {
                    Input = input,
                    UnitPosition = input.Unit.Position,
                    CastPosition = input.Unit.Position,
                    Hitchance = HitChance.VeryHigh
                };
            if ((double)pathLength >= (double)input.Delay * (double)speed - (double)input.RealRadius && (double)Math.Abs(input.Speed - float.MaxValue) < 1.40129846432482E-45)
            {
                float num = input.Delay * speed - input.RealRadius;
                for (int index = 0; index < path.Count - 1; ++index)
                {
                    Vector2 vector2_1 = path[index];
                    Vector2 toVector2 = path[index + 1];
                    float val2 = vector2_1.Distance(toVector2);
                    if ((double)val2 >= (double)num)
                    {
                        Vector2 vector2_2 = (toVector2 - vector2_1).Normalized();
                        Vector2 vector2_3 = vector2_1 + vector2_2 * num;
                        Vector2 vector2_4 = vector2_1 + vector2_2 * (index == path.Count - 2 ? Math.Min(num + input.RealRadius, val2) : num + input.RealRadius);
                        return new PredictionOutput()
                        {
                            Input = input,
                            CastPosition = vector2_3.ToVector3().SetZ(),
                            UnitPosition = vector2_4.ToVector3().SetZ(),
                            Hitchance = GetHitchance(input.Unit)
                        };
                    }
                    num -= val2;
                }
            }
            if ((double)pathLength >= (double)input.Delay * (double)speed - (double)input.RealRadius && (double)Math.Abs(input.Speed - float.MaxValue) > 1.40129846432482E-45)
            {
                float distance = input.Delay * speed - input.RealRadius;
                if ((input.Type == SpellType.Line || input.Type == SpellType.Cone) && Vector3.DistanceSquared(input.From, previousPosition) < Math.Pow(200.0, 2.0))
                    distance = input.Delay * speed;
                path = path.CutPath(distance);
                float delay = 0.0f;
                for (int index = 0; index < path.Count - 1; ++index)
                {
                    Vector2 vector2_1 = path[index];
                    Vector2 vector2_2 = path[index + 1];
                    float num = vector2_1.Distance(vector2_2) / speed;
                    Vector2 vector2_3 = (vector2_2 - vector2_1).Normalized();
                    MovementCollisionInfo movementCollisionInfo = (vector2_1 - speed * delay * vector2_3).VectorMovementCollision(vector2_2, speed, input.From.ToVector2(), input.Speed, delay);
                    float collisionTime = movementCollisionInfo.CollisionTime;
                    Vector2 collisionPosition = movementCollisionInfo.CollisionPosition;
                    if (collisionPosition.IsValid() && (double)collisionTime >= (double)delay && (double)collisionTime <= (double)delay + (double)num)
                    {
                        if (collisionPosition.Distance(vector2_2) >= Math.Sqrt(20d))
                        {
                            Vector2 vector2_4 = collisionPosition - input.RealRadius * vector2_3;
                            return new PredictionOutput()
                            {
                                Input = input,
                                CastPosition = collisionPosition.ToVector3().SetZ(),
                                UnitPosition = vector2_4.ToVector3().SetZ(),
                                Hitchance = GetHitchance(input.Unit)
                            };
                        }
                        break;
                    }
                    delay += num;
                }
            }
            Vector2 vector2 = path.LastOrDefault();
            return new PredictionOutput()
            {
                Input = input,
                CastPosition = vector2.ToVector3().SetZ(),
                UnitPosition = vector2.ToVector3().SetZ(),
                Hitchance = HitChance.Medium
            };
        }

        private static HitChance GetHitchance(IUnit unit) => unit.Type == GameObjectType.AIHeroClient && PathCache.GetPath(unit).Time >= 0.1 ? HitChance.High : HitChance.VeryHigh;
    }

    /*
    using Collision = SparkTech.SDK.MovementPrediction.Collision;

    
    internal class DefaultMovementPrediction : IMovementPrediction
    {
        ModuleMenu IEntropyModule.Menu => null;

        #region Public Methods and Operators

        public PredictionOutput Predict(PredictionInput predInput)
        {
            //TODO make proper return and pred output obj
            var unitPosition = Vector3.Zero;

            var paths = predInput.Target.Path;

            for (var i = 0; i < paths.Length - 1; i++)
            {
                var previousPath = paths[i];
                var currentPath = paths[i + 1];
                var pathLength = previousPath.Distance(currentPath);
                //TODO Use server pos
                var pathPassed = previousPath.Distance(predInput.Target.Position);
                var remainingLength = predInput.Target.Position.Distance(currentPath);
                var pathRatio = pathPassed / pathLength;

                var direction = (currentPath - previousPath).Normalized();
                var velocity = direction * predInput.Target.CharIntermediate.MoveSpeed;

                //TODO Use server pos
                unitPosition = predInput.Target.Position + velocity * predInput.Delay;

                var toUnit = (unitPosition - predInput.From).Normalized();
                var cosTheta = Math.Abs(Vector3.Dot(direction, toUnit)) <= 0 ? 1f : Vector3.Dot(direction, toUnit);

                unitPosition = unitPosition - direction * predInput.Target.BoundingRadius;
                unitPosition = unitPosition - toUnit * predInput.Target.BoundingRadius;

                unitPosition = unitPosition - direction * predInput.Radius;

                var castDirection = (direction + toUnit) * cosTheta;
                unitPosition = unitPosition + castDirection * predInput.Radius;

                var unitDistance = predInput.From.Distance(unitPosition);

                var a = Vector3.Dot(velocity, velocity)
                        - (Math.Abs(predInput.Speed - float.MaxValue) <= 0
                               ? float.MaxValue
                               : (float)Math.Pow(predInput.Speed, 2));

                var b = 2 * predInput.Target.CharIntermediate.MoveSpeed * unitDistance * cosTheta * pathRatio;
                var c = (float)Math.Pow(unitDistance, 2);

                var discriminant = b * b - 4f * a * c;

                if (discriminant < 0)
                {
                    //OutOfRange
                    //0%
                    return new PredictionOutput();
                }

                var impactTime = 2f * c / ((float)Math.Sqrt(discriminant) - b);

                if (impactTime < 0)
                {
                    //None
                    return new PredictionOutput();
                }

                if (remainingLength / predInput.Target.CharIntermediate.MoveSpeed < impactTime)
                {
                    unitPosition = currentPath;
                    break;
                }

                //TODO use server pos
                unitPosition = predInput.Target.Position + velocity * impactTime;

                var checkPosition = unitPosition + direction * predInput.Delay;
                checkPosition = checkPosition + direction * predInput.Target.BoundingRadius;
                if (predInput.From.Distance(checkPosition) > predInput.Range)
                {
                    //TODO add hitchance
                    //OutOfRange
                    //0%
                    return new PredictionOutput();
                }
            }

            var collisionObjects = Collision.GetCollision(
                //TODO ServerPos
                new List<Vector3>
                {
                    predInput.Target.Position, unitPosition
                },
                predInput);

            return new PredictionOutput();
        }

        public PredictionOutput Predict(AIBaseClient target)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }

        #endregion
    }*/
}