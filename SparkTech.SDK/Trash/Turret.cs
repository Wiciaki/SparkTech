namespace SparkTech.SDK.Util.Vector
{
    using System.Linq;

    using SharpDX;

    using SparkTech.SDK.Entities;

    public static class Turret
    {
        #region Constants

        private const float TurretRange = 950f;

        #endregion

        #region Public Methods and Operators
        /*
        public static bool IsUnderTurret(this Vector3 position, ObjectTeam team = ObjectTeam.Enemy)
        {
            return ObjectManager.Get<ITurret>().Any(t => t.Team() == team && position.Distance(t.Position) < TurretRange + t.BoundingRadius);
        }

        public static bool IsUnderTurret(this IAIBase target, GameObjectTeam team = GameObjectTeam.)
        {
            return IsUnderTurret(target.Position, team);
        }*/

        #endregion
    }
}