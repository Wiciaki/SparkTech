namespace SparkTech.SDK.MovementPrediction.Default
{
    using System;
    using System.Collections.Generic;

    using SharpDX;

    using SparkTech.SDK.Util.Vector;
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