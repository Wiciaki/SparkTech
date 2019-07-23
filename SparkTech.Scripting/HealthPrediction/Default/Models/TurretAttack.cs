namespace SparkTech.HealthPrediction.Default.Models
{
    using SparkTech.Utils;

    //TODO Make this inherit AutoAttack
    public class TurretAttack
    {
        #region Enums

        public enum AttackState
        {
            Casted,

            CreatedMissile,

            Completed
        }

        #endregion

        #region Public Properties

        public AttackState AttackStatus { get; set; }

        public float DistanceWobr =>
            this.Target.Distance(this.Sender) - this.Sender.BoundingRadius - this.Target.BoundingRadius;

        //Attack is destroyed
        public bool Inactive { get; set; }

        public MissileClient Missile { get; set; }

        public int MissileCreationTime { get; set; }

        public float PredictedEta => this.PredictedLandTime - Game.TickCount;

        public float PredictedLandTime => this.StartTime + this.Sender.AttackCastDelay * 1000
                                          + this.DistanceWobr / this.Sender.BasicAttack.MissileSpeed * 1000
                                          - (float)EnetClient.Ping / 2;

        public float PredictedMissileEta
        {
            get
            {
                if (this.Missile != null && this.Missile.IsValid)
                {
                    //var position = this.Missile.ServerPosition;
                    var distance = this.Missile.Distance(this.Target);

                    var travelTime = distance / this.Sender.BasicAttack.MissileSpeed * 1000 - EnetClient.Ping / 2f;

                    return travelTime;
                }

                return this.PredictedEta;
            }
        }

        public int RealEndTime { get; set; }

        public AITurret Sender { get; set; }

        public int StartTime { get; set; }

        public AIBaseClient Target { get; set; }

        #endregion
    }
}