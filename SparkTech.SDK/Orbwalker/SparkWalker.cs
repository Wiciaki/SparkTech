namespace SparkTech.SDK.Orbwalker
{
    using System;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.EventData;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.Properties;
    using SparkTech.SDK.Rendering;

    public class SparkWalker : Orbwalker
    {
        public override Menu Menu { get; }

        public override JObject GetTranslations()
        {
            return JObject.Parse(Resources.Orbwalker);
        }

        public SparkWalker()
        {
            this.Menu = new Menu("spark")
            {
                new MenuText("info"),
                new Menu("speed")
                {
                    new MenuList("logic"),
                    new MenuFloat("move", -0.3f, 0.6f, 0f),
                    new MenuFloat("attack", 0f, 0.15f, 0.04f),
                    new MenuFloat("orderTime", 0f, 0.15f, 0.04f),
                },
                new Menu("drawings")
                {
                    new MenuColorBool("playerRange", Color.DodgerBlue, true),
                    CreateEnemyMenu(),
                    new MenuColorBool("minions", Color.Silver, true),
                    new MenuColorBool("playerHold", Color.DodgerBlue, false),
                    new MenuColorBool("targetMinion", Color.Magenta, false)
                },
                new MenuBool("underTurretFarming", true),
                new MenuBool("meleeMagnet", false)
            };

            if (Platform.HasCoreAPI)
            {
                this.Humanizer = new Humanizer();

                EntityEvents.OnDoCast += this.DoCast;
                EntityEvents.OnProcessSpellCast += this.ProcessSpellCast;
                EntityEvents.OnSpellbookStopCast += this.SpellbookStopCast;
            }
        }

        public override void Start()
        {
            Game.OnUpdate += this.OnUpdate;
            Render.OnDraw += this.OnDraw;
        }

        public override void Pause()
        {
            Game.OnUpdate -= this.OnUpdate;
            Render.OnDraw -= this.OnDraw;
        }

        private static Menu CreateEnemyMenu()
        {
            var menu = new Menu("enemy") { new MenuText("alone") };
            menu.IsSaving = false;

            if (Platform.HasCoreAPI)
            {
                foreach (var hero in ObjectManager.Get<IHero>().Where(h => h.IsEnemy() && !h.IsDummy()))
                {
                    menu.Add(new MenuColorBool(GetMenuId(hero), Color.Yellow, false));
                    menu["alone"].IsVisible = false;
                }
            }

            return menu;
        }

        private static string GetMenuId(IHero hero)
        {
            return $"{hero.CharName} [{hero.Name}/{hero.Id}]";
        }

        private void OnDraw()
        {
            var drawings = this.Menu.GetMenu("drawings");
            var playerRange = drawings["playerRange"];

            if (playerRange.GetValue<bool>())
            {
                Circle.Draw(playerRange.GetValue<Color>(), Orbwalking.GetAutoAttackRange(ObjectManager.Player), ObjectManager.Player.Position);
            }

            foreach (var (hero, color) in from hero in ObjectManager.Get<IHero>() where hero.IsEnemy() && hero.IsVisible let item = drawings[GetMenuId(hero)] where item != null && item.GetValue<bool>() select (hero, item.GetValue<Color>()))
            {
                Circle.Draw(color, Orbwalking.GetAutoAttackRange(hero), hero.Position);
            }

            var playerHold = drawings["playerHold"];

            if (playerHold.GetValue<bool>())
            {
                Circle.Draw(playerHold.GetValue<Color>(), ObjectManager.Player.BoundingRadius, ObjectManager.Player.Position);
            }
        }

        private float attackT;

        protected readonly Humanizer Humanizer;

        protected virtual IUnit Unit => ObjectManager.Player;

        protected virtual GameObjectOrder AttackOrder => GameObjectOrder.AttackUnit;

        protected virtual GameObjectOrder MoveOrder => GameObjectOrder.MoveTo;

        protected virtual GameObjectOrder StopOrder => GameObjectOrder.Stop;

        protected float GetPingOffset() => Game.Ping / 2000f;

        private void SpellbookStopCast(StopCastEventArgs args)
        {
            if (this.Unit.Compare(args.Source?.Owner) && args.DestroyMissile && args.KeepAnimationPlaying)
            {
                this.attackT = 0;
            }
        }

        private void ProcessSpellCast(ProcessSpellCastEventArgs args)
        {
            if (this.Unit.Compare(args.Source) && Orbwalking.IsAutoAttack(args.SpellData.Name))
            {
                ProcessAfterAttack((IAttackable)args.Target);
            }
        }

        private void DoCast(ProcessSpellCastEventArgs args)
        {
            if (!this.Unit.Compare(args.Source))
            {
                return;
            }

            var name = args.SpellData.Name;

            if (Orbwalking.IsAutoAttackReset(name))
            {
                this.attackT = 0;
            }
            else if (Orbwalking.IsAutoAttack(name))
            {
                this.attackT = Game.Time - GetPingOffset();
            }
        }

        public bool CanAttack(float time)
        {
            var attackSetting = Menu.GetMenu("speed")["attack"].GetValue<float>();

            return time + GetPingOffset() + attackSetting >= this.attackT + this.Unit.AttackDelay;
        }

        public bool CanMove(float time)
        {
            var menu = Menu.GetMenu("speed");
            var logic = menu["logic"].GetValue<int>();
            
            time += GetPingOffset() + menu["move"].GetValue<float>();

            var attackCastDelay = this.Unit.AttackCastDelay;

            if (logic != 0)
            {
                if (time >= this.attackT + attackCastDelay)
                {
                    return true;
                }

                if (logic == 2)
                {
                    return false;
                }
            }

            var attackSpeedMod = this.Unit.AttackSpeedMod / 10f;
            var attackSpeed = 0.1f / this.Unit.AttackDelay;
            var percentAttackSpeedMod = this.Unit.PercentAttackSpeedMod / 10f;

            var magic = (attackSpeed + attackSpeedMod) * percentAttackSpeedMod * attackSpeed * 2f + percentAttackSpeedMod;

            return time + magic >= this.attackT + attackCastDelay * (1f - attackSpeedMod);
        }

        private void OnUpdate(EventArgs obj)
        {
            var mode = Mode.Current;

            if (mode.IsNone)
            {
                return;
            }

            var time = Game.Time;

            if (CanAttack(time))
            {
                var target = this.GetOrbwalkingTarget(mode);

                if (target != null && ProcessBeforeAttack(target) && this.Humanizer.IssueOrder(this.AttackOrder, target))
                {
                    this.attackT = time + this.Unit.AttackDelay;
                }
            }

            var extraTime = Menu.GetMenu("speed")["orderTime"].GetValue<float>();

            if (CanMove(time) && this.attackT <= time + extraTime)
            {
                this.Humanizer.IssueOrder(this.MoveOrder, Game.Cursor);
            }
        }

        private IAttackable GetOrbwalkingTarget(Mode mode)
        {
            return ObjectManager.Get<IAttackable>().Where(t => t.IsValidTarget() && t.Distance(this.Unit) <= Orbwalking.GetAutoAttackRange(this.Unit, t)).OrderByDescending(h => h is IHero).FirstOrDefault();
        }
    }
}