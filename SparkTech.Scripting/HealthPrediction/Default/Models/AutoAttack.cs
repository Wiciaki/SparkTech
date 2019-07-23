namespace SparkTech.HealthPrediction.Default.Models
{
    using System;

    using SparkTech.Spells;
    using SparkTech.Utils;

    using SharpDX;

    public abstract class AutoAttack
    {
        #region Constructors and Destructors

        protected AutoAttack(AIBaseClient sender, AIBaseClient target)
        {
            this.AttackStatus = AttackState.Detected;
            this.DetectTime = Game.TickCount - (float)EnetClient.Ping / 2;
            this.Sender = sender;
            this.Target = target;
            this.SenderNetworkID = sender.NetworkID;
            this.NetworkID = target.NetworkID;

            this.Damage = sender.GetAutoAttackDamage(target);

            //TODO Server pos
            this.StartPosition = sender.Position;

            this.AnimationDelay = (int)(sender.AttackCastDelay * 1000);
        }

        #endregion

        #region Enums

        [Flags]
        public enum AttackState
        {
            None,

            Detected,

            Completed
        }

        #endregion

        #region Public Properties

        public float AnimationDelay { get; set; }

        public AttackState AttackStatus { get; set; }

        public bool CanRemoveAttack => Game.TickCount - this.DetectTime > 2000;

        public float Damage { get; set; }

        public float DetectTime { get; set; }

        public float Distance => this.StartPosition.Distance(this.Target.Position) - this.Sender.BoundingRadius
                                 - this.Target.BoundingRadius;

        public float ElapsedTime => Game.TickCount - this.DetectTime;

        public virtual float ETA => this.LandTime - Game.TickCount;

        public uint ExtraDelay { get; set; }

        public abstract float LandTime { get; }

        public uint NetworkID { get; set; }

        public AIBaseClient Sender { get; set; }

        public uint SenderNetworkID { get; set; }

        public Vector3 StartPosition { get; set; }

        public AIBaseClient Target { get; set; }

        #endregion

        #region Public Methods and Operators

        public abstract bool HasReached();

        public virtual bool IsValid()
        {
            return this.Sender.IsValid && this.Target.IsValid;
        }

        #endregion
    }
}