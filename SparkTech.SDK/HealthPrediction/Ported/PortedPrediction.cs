namespace SparkTech.SDK.HealthPrediction.Ported
{
    /*
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.EventArgs;
    using SparkTech.SDK.GUI.Menu;

    public class PortedPrediction : IHealthPredition
    {
        public PortedPrediction()
        {
            EntityEvents.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
            EntityEvents.OnSpellbookStopCast += SpellbookOnStopCast;
            ObjectManager.OnDelete += MissileClient_OnDelete;
            EntityEvents.OnDoCast += Obj_AI_Base_OnDoCast;
        }

        public void Release()
        {

        }

        public Menu Menu { get; }

        public float Predict(IUnit unit, float time)
        {
            return GetHealthPrediction(unit, time);
        }

        #region Static Fields

        /// <summary>
        ///     The active attacks
        /// </summary>
        private static readonly Dictionary<int, PredictedDamage> ActiveAttacks = new Dictionary<int, PredictedDamage>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Return the Attacking turret.
        /// </summary>
        /// <param name="minion"></param>
        /// <returns></returns>
        public static IUnit GetAggroTurret(IMinion minion)
        {
            var ActiveTurret =
                ActiveAttacks.Values.FirstOrDefault(
                    m => (m.Source is ITurret) && m.Target.Id == minion.Id);

            return ActiveTurret?.Source;
        }

        /// <summary>
        ///     Returns the unit health after a set time milliseconds.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="time">The time.</param>
        /// <param name="delay">The delay.</param>
        /// <returns></returns>
        public static float GetHealthPrediction(IUnit unit, int time, int delay = 70)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values)
            {
                var attackDamage = 0f;
                if (!attack.Processed && attack.Source.IsValidTarget(float.MaxValue, false)
                    && attack.Target.IsValidTarget(float.MaxValue, false) && attack.Target.NetworkId == unit.NetworkId)
                {
                    var landTime = attack.StartTick + attack.Delay
                                   + 1000 * Math.Max(0, unit.Distance(attack.Source) - attack.Source.BoundingRadius)
                                   / attack.ProjectileSpeed + delay;

                    if ( //Utils.GameTimeTickCount < landTime - delay &&
    landTime < Utils.GameTimeTickCount + time)
                    {
                        attackDamage = attack.Damage;
                    }
                }

                predictedDamage += attackDamage;
            }

            return unit.Health - predictedDamage;
        }

        /// <summary>
        ///     Determines whether the specified minion has minion aggro.
        /// </summary>
        /// <param name="minion">The minion.</param>
        /// <returns></returns>
        public static bool HasMinionAggro(IMinion minion)
        {
            return ActiveAttacks.Values.Any(m => (m.Source is IMinion) && m.Target.Id == minion.Id);
        }

        /// <summary>
        ///     Determines whether the specified minion has turret aggro.
        /// </summary>
        /// <param name="minion">The minion</param>
        /// <returns></returns>
        public static bool HasTurretAggro(IMinion minion)
        {
            return ActiveAttacks.Values.Any(m => (m.Source is ITurret) && m.Target.Id == minion.Id);
        }

        /// <summary>
        ///     Returns the unit health after time milliseconds assuming that the past auto-attacks are periodic.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="time">The time.</param>
        /// <param name="delay">The delay.</param>
        /// <returns></returns>
        public static float LaneClearHealthPrediction(IUnit unit, int time, int delay = 70)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values)
            {
                var n = 0;
                if (Utils.GameTimeTickCount - 100 <= attack.StartTick + attack.AnimationTime
                    && attack.Target.IsValidTarget(float.MaxValue, false)
                    && attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.NetworkId == unit.NetworkId)
                {
                    var fromT = attack.StartTick;
                    var toT = Utils.GameTimeTickCount + time;

                    while (fromT < toT)
                    {
                        if (fromT >= Utils.GameTimeTickCount
                            && (fromT + attack.Delay
                                + Math.Max(0, unit.Distance(attack.Source) - attack.Source.BoundingRadius)
                                / attack.ProjectileSpeed < toT))
                        {
                            n++;
                        }
                        fromT += (int)attack.AnimationTime;
                    }
                }
                predictedDamage += n * attack.Damage;
            }

            return unit.Health - predictedDamage;
        }

        /// <summary>
        ///     Return the starttick of the attacking turret.
        /// </summary>
        /// <param name="minion"></param>
        /// <returns></returns>
        public static int TurretAggroStartTick(IMinion minion)
        {
            var ActiveTurret =
                ActiveAttacks.Values.FirstOrDefault(
                    m => (m.Source is ITurret) && m.Target.Id == minion.Id);

            return ActiveTurret != null ? ActiveTurret.StartTick : 0;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            ActiveAttacks.ToList()
                .Where(pair => pair.Value.StartTick < Utils.GameTimeTickCount - 3000)
                .ToList()
                .ForEach(pair => ActiveAttacks.Remove(pair.Key));
        }

        /// <summary>
        ///     Fired when a <see cref="MissileClient" /> is deleted from the game.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        static void MissileClient_OnDelete(IGameObject sender)
        {
            var missile = sender as IMissile;

            if (missile != null && missile.SpellCaster != null)
            {
                var casterNetworkId = missile.SpellCaster.NetworkId;
                foreach (var activeAttack in ActiveAttacks)
                {
                    if (activeAttack.Key == casterNetworkId)
                    {
                        ActiveAttacks[casterNetworkId].Processed = true;
                    }
                }
            }
        }

        /// <summary>
        ///     Fired when a unit does an auto attack.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void Obj_AI_Base_OnDoCast(ProcessCastEventArgs args)
        {
            var sender = args.Source;

            if (ActiveAttacks.ContainsKey(sender.Id) && sender.IsMelee)
            {
                ActiveAttacks[sender.Id].Processed = true;
            }
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void ObjAiBaseOnOnProcessSpellCast(ProcessCastEventArgs args)
        {
            var sender = args.Source;

            if (!sender.IsValidTarget(3000, false) || sender.Team != ObjectManager.Player.Team || sender is IHero
                || !Orbwalking.IsAutoAttack(args.SData.Name) || !(args.Target is Obj_AI_Base))
            {
                return;
            }

            var target = (IUnit)args.Target;
            ActiveAttacks.Remove(sender.Id);

            var attackData = new PredictedDamage(
                sender,
                target,
                Game.Time,
                sender.AttackCastDelay * 1000,
                sender.AttackDelay * 1000 - (sender is ITurret ? 70 : 0),
                sender.IsMelee() ? int.MaxValue : (int)args.SData.MissileSpeed,
                (float)sender.GetAutoAttackDamage(target, true));
            ActiveAttacks.Add(sender.NetworkId, attackData);
        }

        /// <summary>
        ///     Fired when the spellbooks stops a cast.
        /// </summary>
        /// <param name="args">The <see cref="SpellbookStopCastEventArgs" /> instance containing the event data.</param>
        private static void SpellbookOnStopCast(StopCastEventArgs args)
        {
            var owner = args.Source.Owner;

            if (owner.IsValid && args.StopAnimation)
            {
                if (ActiveAttacks.ContainsKey(owner.Id))
                {
                    ActiveAttacks.Remove(owner.Id);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Represetns predicted damage.
        /// </summary>
        private class PredictedDamage
        {
            #region Fields

            /// <summary>
            ///     The animation time
            /// </summary>
            public readonly float AnimationTime;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="PredictedDamage" /> class.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <param name="target">The target.</param>
            /// <param name="startTime">The start tick.</param>
            /// <param name="delay">The delay.</param>
            /// <param name="animationTime">The animation time.</param>
            /// <param name="projectileSpeed">The projectile speed.</param>
            /// <param name="damage">The damage.</param>
            public PredictedDamage(
                IUnit source,
                IUnit target,
                float startTime,
                float delay,
                float animationTime,
                int projectileSpeed,
                float damage)
            {
                this.Source = source;
                this.Target = target;
                this.StartTime = startTime;
                this.Delay = delay;
                this.ProjectileSpeed = projectileSpeed;
                this.Damage = damage;
                this.AnimationTime = animationTime;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the damage.
            /// </summary>
            /// <value>
            ///     The damage.
            /// </value>
            public float Damage { get; private set; }

            /// <summary>
            ///     Gets or sets the delay.
            /// </summary>
            /// <value>
            ///     The delay.
            /// </value>
            public float Delay { get; private set; }

            /// <summary>
            ///     Gets or sets a value indicating whether this <see cref="PredictedDamage" /> is processed.
            /// </summary>
            /// <value>
            ///     <c>true</c> if processed; otherwise, <c>false</c>.
            /// </value>
            public bool Processed { get; internal set; }

            /// <summary>
            ///     Gets or sets the projectile speed.
            /// </summary>
            /// <value>
            ///     The projectile speed.
            /// </value>
            public int ProjectileSpeed { get; private set; }

            /// <summary>
            ///     Gets or sets the source.
            /// </summary>
            /// <value>
            ///     The source.
            /// </value>
            public IUnit Source { get; private set; }

            /// <summary>
            ///     Gets or sets the start tick.
            /// </summary>
            /// <value>
            ///     The start tick.
            /// </value>
            public float StartTime { get; internal set; }

            /// <summary>
            ///     Gets or sets the target.
            /// </summary>
            /// <value>
            ///     The target.
            /// </value>
            public IUnit Target { get; private set; }

            #endregion
        }
    }*/
}