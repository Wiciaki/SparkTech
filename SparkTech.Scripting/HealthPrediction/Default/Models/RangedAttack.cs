namespace SparkTech.HealthPrediction.Default.Models
{
    using SparkTech.Geometry;
    using SparkTech.Utils;

    using SharpDX;

    public class RangedAttack : AutoAttack
    {
        #region Constructors and Destructors

        public RangedAttack(AIBaseClient sender, AIBaseClient target, MissileClient mc, uint extraDelay)
            : base(sender, target)
        {
            this.ExtraDelay = extraDelay;
            this.Missile = mc;
            this.StartPosition = mc.Position;
        }

        #endregion

        #region Public Properties

        public Vector3 EstimatedPosition
        {
            get
            {
                var dist = this.Missile.SpellData.MissileSpeed / 1000 * (Game.TickCount - this.DetectTime);
                return this.StartPosition.Extend(this.Missile.EndPosition, dist);
            }
        }

        public override float LandTime => Game.TickCount + this.TimeToLand;

        public MissileClient Missile { get; set; }

        //isvalidcheckbefore
        //TODO Server pos
        public float TimeToLand => this.Missile.Position.Distance(this.Missile.EndPosition)
                                   / this.Missile.SpellData.MissileSpeed * 1000;

        #endregion

        #region Public Methods and Operators

        public override bool HasReached()
        {
            return this.Missile == null || !this.IsValid() || this.AttackStatus == AttackState.Completed
                   || this.ETA < -200;
        }

        public override bool IsValid()
        {
            return this.Missile.IsValid && this.Sender.IsValid && this.Target.IsValid;
        }

        #endregion
    }
}