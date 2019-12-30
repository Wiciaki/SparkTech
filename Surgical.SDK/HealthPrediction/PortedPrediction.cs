namespace Surgical.SDK.HealthPrediction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using Surgical.SDK.Entities;
    using Surgical.SDK.EventData;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.Modules;

    public class PortedPrediction : IHealthPredition
    {
        public PortedPrediction()
        {
            if (!Platform.HasCoreAPI)
            {
                return;
            }

            EntityEvents.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
            EntityEvents.OnSpellbookStopCast += SpellbookOnStopCast;
            ObjectManager.OnDelete += MissileClient_OnDelete;
            EntityEvents.OnDoCast += Obj_AI_Base_OnDoCast;
        }

        Menu IModule.Menu { get; } = new Menu("surgical");

        JObject? IModule.GetTranslations()
        {
            return null;
        }

        void IResumable.Start()
        {

        }

        void IResumable.Pause()
        {

        }

        public float Predict(IUnit unit, float time)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values)
            {
                var attackDamage = 0f;

                //if (!attack.Processed && attack.Source.IsValidTarget(float.MaxValue, false)
                //                      && attack.Target.IsValidTarget(float.MaxValue, false) && attack.Target.Id == unit.Id)
                {
                    var landTime = attack.StartTime + attack.Delay
                                                    + Math.Max(0, unit.Distance(attack.Source) - attack.Source.BoundingRadius)
                                                    / attack.ProjectileSpeed /* + delay */;

                    if ( //Utils.GameTimeTickCount < landTime - delay &&
                        landTime < Game.Time + time)
                    {
                        attackDamage = attack.Damage;
                    }
                }

                predictedDamage += attackDamage;
            }

            return unit.Health - predictedDamage;
        }

        #region Static Fields

        /// <summary>
        ///     The active attacks
        /// </summary>
        private static readonly Dictionary<int, PredictedDamage> ActiveAttacks = new Dictionary<int, PredictedDamage>();

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            ActiveAttacks.Where(pair => pair.Value.StartTime < Game.Time).ToList().ForEach(pair => ActiveAttacks.Remove(pair.Key));
        }

        /// <summary>
        ///     Fired when a <see cref="MissileClient" /> is deleted from the game.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        static void MissileClient_OnDelete(IGameObject sender)
        {
            if (sender is IMissile missile && missile.Caster != null)
            {
                var id = missile.Caster.Id;

                if (ActiveAttacks.Any(attack => attack.Key == id))
                {
                    ActiveAttacks[id].Processed = true;
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

            if (ActiveAttacks.ContainsKey(sender.Id) && sender.CombatType == GameObjectCombatType.Melee)
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

            //if (!sender.IsValidTarget(3000, false) || sender.Team != ObjectManager.Player.Team || sender is IHero
            //    || !Orbwalking.IsAutoAttack(args.Spell.Name) || !(args.Target is IUnit))
            //{
            //    return;
            //}

            //var target = (IUnit)args.Target;
            //ActiveAttacks.Remove(sender.Id);

            //var attackData = new PredictedDamage(
            //    sender,
            //    target,
            //    Game.Time,
            //    sender.AttackCastDelay * 1000,
            //    sender.AttackDelay * 1000 - (sender is ITurret ? 70 : 0),
            //    sender.CombatType == GameObjectCombatType.Melee ? int.MaxValue : (int)args.Spell.MissileSpeed,
            //    (float)sender.GetAutoAttackDamage(target, true));

            //ActiveAttacks.Add(sender.Id, attackData);
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
    }
}