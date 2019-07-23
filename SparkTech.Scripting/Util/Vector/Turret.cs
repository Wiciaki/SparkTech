namespace SparkTech.Utils
{
    using System.Linq;

    using SparkTech.Caching;
    using SparkTech.Enumerations;

    using SharpDX;

    public static class Turret
    {
        #region Constants

        private const float TurretRange = 950f;

        #endregion

        #region Public Methods and Operators

        public static bool IsUnderTurret(this Vector3 position, ObjectTeam team = ObjectTeam.Enemy)
        {
            return ObjectCache.Get<AITurret>().Any(
                t => t.Team() == team && position.Distance(t.Position) < TurretRange + t.BoundingRadius);
        }

        public static bool IsUnderTurret(this AIBaseClient target, ObjectTeam team = ObjectTeam.Enemy)
        {
            return IsUnderTurret(target.Position, team);
        }

        #endregion
    }
}