namespace SparkTech.HealthPrediction.Default.Models
{
    public class MeleeAttack : AutoAttack
    {
        #region Constructors and Destructors

        public MeleeAttack(AIBaseClient sender, AIBaseClient target, uint extraDelay)
            : base(sender, target)
        {
            this.ExtraDelay = extraDelay + 100;
        }

        #endregion

        #region Public Properties

        public override float LandTime => this.DetectTime + this.Sender.AttackCastDelay * 1000 + this.ExtraDelay;

        #endregion

        #region Public Methods and Operators

        public override bool HasReached()
        {
            return this.AttackStatus == AttackState.Completed || this.ETA < -100;
        }

        #endregion
    }
}