namespace SparkTech.SDK.HealthPrediction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.EventData;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.Properties;
    using Rendering;
    using SharpDX;
    using SparkTech.SDK.DamageLibrary;

    public class PortedPrediction : IHealthPredition
    {
        public PortedPrediction()
        {
            if (!Platform.HasCoreAPI)
            {
                return;
            }

            EntityEvents.OnProcessSpellCast += OnObjAiBaseProcessSpellCast;
            Game.OnUpdate += OnGameUpdate;
            EntityEvents.OnSpellbookStopCast += OnSpellbookStopCast;
            ObjectManager.OnDelete += OnGameObjectDelete;
            EntityEvents.OnDoCast += OnObjAiBaseDoCast;
            Render.OnDraw += this.Render_OnDraw;
        }

        private void Render_OnDraw()
        {
            foreach (var unit in ActiveAttacks.Select(p => p.Value.Target).Distinct())
            {
                var damage = this.Predict(unit, 2f);

                Text.Draw(damage.ToString(), Color.Magenta, (Point)Game.WorldToScreen(unit.Position));
            }
        }

        public Menu Menu { get; } = new Menu("spark") { new MenuInt("windup", -20, 120, 0) };

        private float Windup => this.Menu["windup"].GetValue<int>();

        public JObject GetTranslations()
        {
            return JObject.Parse(Resources.HealthPrediction);
        }

        public void Start()
        {

        }

        public void Pause()
        {

        }

        private static readonly Dictionary<int, PredictedDamage> ActiveAttacks = new Dictionary<int, PredictedDamage>();

        private static int TickCount => (int)(Game.Time * 1000f);

        /// <summary>
        ///     Last Tick Update
        /// </summary>
        private static int lastTick;

        #region Public Methods and Operators

        /// <summary>
        ///     Return the Attacking turret.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static IUnit GetAggroTurret(IUnit minion)
        {
            var activeTurret =
                ActiveAttacks.Values.FirstOrDefault(m => m.Source is IUnit && m.Target.Compare(minion));
            return activeTurret?.Source;
        }

        /// <summary>
        ///     Determines whether the specified minion has turret aggro.
        /// </summary>
        /// <param name="minion">The minion</param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static bool HasTurretAggro(IMinion minion)
        {
            return ActiveAttacks.Values.Any(m => m.Source is IMinion && m.Target.Compare(minion));
        }

        /// <summary>
        ///     Return the starttick of the attacking turret.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static int TurretAggroStartTick(IMinion minion)
        {
            var activeTurret =
                ActiveAttacks.Values.FirstOrDefault(m => m.Source is ITurret && m.Target.Compare(minion));
            return activeTurret?.StartTick ?? 0;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the default prediction of the unit.
        /// </summary>
        /// <param name="unit">
        ///     The unit
        /// </param>
        /// <param name="time">
        ///     The time
        /// </param>
        /// <param name="delay">
        ///     The delay
        /// </param>
        /// <returns>
        ///     The <see cref="float" />
        /// </returns>
        public float Predict(IUnit unit, float time)
        {
            var predictedDamage = 0f;
            foreach (var attack in ActiveAttacks.Values.Where(i => i.Target.Compare(unit) && !i.Processed))
            {
                var attackDamage = 0f;
                if (attack.Source.IsValidTarget(false) && attack.Target.IsValidTarget())
                {
                    var landTime = attack.StartTick + attack.Delay
                                   + 1000
                                   * (attack.Source.CombatType == GameObjectCombatType.Melee
                                          ? 0
                                          : Math.Max(0, unit.Distance(attack.Source) - attack.Source.BoundingRadius)
                                            / attack.ProjectileSpeed) + this.Windup;
                    if (landTime < TickCount + time*1000)
                    {
                        attackDamage = attack.Damage;
                    }
                }

                predictedDamage += attackDamage;
            }

            return unit.Health - predictedDamage;
        }

        /// <summary>
        ///     GameObject on delete subscribed event function.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="GameObject" /> sender
        /// </param>
        /// <param name="args">
        ///     <see cref="System.EventArgs" /> event data
        /// </param>
        private static void OnGameObjectDelete(IGameObject sender)
        {
            if (!sender.IsValid)
            {
                return;
            }

            var aiBase = sender as IUnit;
            if (aiBase != null)
            {
                var objNetworkId = aiBase.Id;
                if (ActiveAttacks.ContainsKey(objNetworkId))
                {
                    ActiveAttacks.Remove(objNetworkId);
                    return;
                }
                foreach (var activeAttack in ActiveAttacks.Values.Where(i => i.Target.Compare(aiBase)))
                {
                    ActiveAttacks.Remove(activeAttack.Source.Id);
                }
                return;
            }

            var missile = sender as IMissile;
            if (missile?.Caster != null)
            {
                var casterNetworkId = missile.Caster.Id;
                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks[casterNetworkId].Processed = true;
                }
            }
        }

        /// <summary>
        ///     Game Tick which is called by the game update event.
        /// </summary>
        /// <param name="args">
        ///     <see cref="System.EventArgs" /> event data
        /// </param>
        private static void OnGameUpdate(EventArgs args)
        {
            if (TickCount - lastTick <= 1000)
            {
                return;
            }

            ActiveAttacks.ToList()
                .Where(pair => pair.Value.StartTick < TickCount - 3000)
                .ToList()
                .ForEach(pair => ActiveAttacks.Remove(pair.Key));

            lastTick = TickCount;
        }

        /// <summary>
        ///     Obj_AI_Base on DoCast subscribed event function.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="Obj_AI_Base" /> sender
        /// </param>
        /// <param name="args">
        ///     <see cref="GameObjectProcessSpellCastEventArgs" /> event data
        /// </param>
        private static void OnObjAiBaseDoCast(ProcessSpellCastEventArgs args)
        {
            var sender = args.Source;

            if (sender.IsValid && sender.CombatType == GameObjectCombatType.Melee)
            {
                var casterNetworkId = sender.Id;
                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks[casterNetworkId].Processed = true;
                }
            }
        }

        /// <summary>
        ///     Process Spell Cast subscribed event function.
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void OnObjAiBaseProcessSpellCast(ProcessSpellCastEventArgs args)
        {
            var sender = args.Source;
            if (!sender.IsValidTarget(false) ||/* !AutoAttack.IsAutoAttack(args.SData.Name) ||*/ !sender.IsAlly())
            {
                return;
            }

            if (!(sender is IMinion) && !(sender is ITurret))
            {
                return;
            }

            var target = args.Target as IMinion;

            if (target == null)
            {
                return;
            }

            ActiveAttacks.Remove(sender.Id);
            ActiveAttacks.Add(
                sender.Id,
                new PredictedDamage(
                    sender,
                    target,
                    (int)((Game.Time*1000) - (Game.Ping / 2)),
                    sender.AttackCastDelay * 1000,
                    (sender.AttackDelay * 1000) - (sender is ITurret ? 70 : 0),
                    sender.CombatType == GameObjectCombatType.Melee ? int.MaxValue : (int)args.SpellData.MissileSpeed,
                    sender.GetAutoAttackDamage(target)));
        }

        /// <summary>
        ///     Spell-book on casting stop subscribed event function.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="Spellbook" /> sender
        /// </param>
        /// <param name="args">Spell-book Stop Cast Data</param>
        private static void OnSpellbookStopCast(StopCastEventArgs args)
        {
            var sender = args.Source;
            if (sender.Owner.IsValid && !args.KeepAnimationPlaying && args.DestroyMissile)
            {
                var casterNetworkId = sender.Owner.Id;
                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks.Remove(casterNetworkId);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Predicted Damage Container
        /// </summary>
        private class PredictedDamage
        {
            #region Fields

            /// <summary>
            ///     Animation Time
            /// </summary>
            public readonly float AnimationTime;

            /// <summary>
            ///     The Damage
            /// </summary>
            public readonly float Damage;

            /// <summary>
            ///     Delay before damage impact
            /// </summary>
            public readonly float Delay;

            /// <summary>
            ///     Projectile Speed
            /// </summary>
            public readonly int ProjectileSpeed;

            /// <summary>
            ///     The Source
            /// </summary>
            public readonly IUnit Source;

            /// <summary>
            ///     Start Tick
            /// </summary>
            public readonly int StartTick;

            /// <summary>
            ///     The Target
            /// </summary>
            public readonly IUnit Target;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="PredictedDamage" /> class.
            /// </summary>
            /// <param name="source">
            ///     Damage Source
            /// </param>
            /// <param name="target">
            ///     Damage Target
            /// </param>
            /// <param name="startTick">
            ///     Starting Game Tick
            /// </param>
            /// <param name="delay">
            ///     Delay of damage impact
            /// </param>
            /// <param name="animationTime">
            ///     Animation time
            /// </param>
            /// <param name="projectileSpeed">
            ///     Projectile Speed
            /// </param>
            /// <param name="damage">
            ///     The Damage
            /// </param>
            public PredictedDamage(
                IUnit source,
                IUnit target,
                int startTick,
                float delay,
                float animationTime,
                int projectileSpeed,
                float damage)
            {
                this.Source = source;
                this.Target = target;
                this.StartTick = startTick;
                this.Delay = delay;
                this.ProjectileSpeed = projectileSpeed;
                this.Damage = damage;
                this.AnimationTime = animationTime;
            }

            #endregion

            #region Public Properties

            public bool Processed { get; set; }

            #endregion
        }
    }
}