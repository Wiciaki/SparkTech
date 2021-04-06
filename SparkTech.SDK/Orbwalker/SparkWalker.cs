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
        public SparkWalker()
        {
            this.Menu = new Menu("spark")
            {
                new MenuText("info"),
                new Menu("speed")
                {
                    new MenuList("logic"),
                    new MenuFloat("move", -0.3f, 1.5f, 0f),
                    new MenuFloat("attack", 0f, 0.15f, 0.04f),
                    new MenuFloat("orderTime", 0f, 0.15f, 0.04f),
                },
                new Menu("drawings")
                {
                    new MenuColorBool("playerRange", Color.DodgerBlue, true),
                    GetEnemyMenu(),
                    new MenuColorBool("minions", Color.Silver, true),
                    new MenuColorBool("playerHold", Color.DodgerBlue, false),
                    new MenuColorBool("targetMinion", Color.Magenta, false)
                },
                new MenuBool("underTurretFarming", true),
                new MenuBool("meleeMagnet", false)
            };

            var basic = this.Menu.GetMenu("speed")["logic"];
            ShowItems(basic.GetValue<int>());
            basic.BeforeValueChange += args => ShowItems(args.NewValue<int>());

            if (Platform.HasCoreAPI)
            {
                EntityEvents.OnDoCast += this.OnDoCast;
                EntityEvents.OnProcessSpellCast += this.OnProcessSpellCast;
                EntityEvents.OnSpellbookStopCast += this.OnSpellbookStopCast;
            }
        }

        private void ShowItems(int value)
        {
            this.Menu.GetMenu("speed")["move"].IsVisible = value != 2;
        }

        public override void Start()
        {
            if (Platform.HasCoreAPI)
            {
                Game.OnUpdate += this.OnUpdate;
                Render.OnDraw += this.OnDraw;
            }
        }

        public override void Pause()
        {
            if (Platform.HasCoreAPI)
            {
                Game.OnUpdate -= this.OnUpdate;
                Render.OnDraw -= this.OnDraw;
            }
        }

        public override Menu Menu { get; }

        public override JObject GetTranslations()
        {
            return JObject.Parse(Resources.Orbwalker);
        }

        private static Menu GetEnemyMenu()
        {
            var menu = new Menu("enemy") { new MenuText("alone") };
            menu.IsSaving = false;

            if (Platform.HasCoreAPI)
            {
                foreach (var hero in ObjectManager.Get<IHero>().Where(h => h.IsEnemy() && h.CharName != "PracticeTool_TargetDummy"))
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

        protected virtual IUnit Unit => ObjectManager.Player;

        private void OnSpellbookStopCast(StopCastEventArgs args)
        {
            var owner = args.Source?.Owner;

            if (owner == null || owner.Id != this.Unit.Id || !args.DestroyMissile || !args.KeepAnimationPlaying)
            {
                return;
            }

            this.attackT = 0;
        }

        private void OnProcessSpellCast(ProcessSpellCastEventArgs args)
        {
            if (args.Source.Id != this.Unit.Id)
            {
                return;
            }

            var name = args.SpellData.Name;

            if (Orbwalking.IsAutoAttackReset(name))
            { 
                this.attackT = 0; 
            }

            if (Orbwalking.IsAutoAttack(name))
            {
                ProcessAfterAttack((IAttackable)args.Target);
            }
        }

        private void OnDoCast(ProcessSpellCastEventArgs args)
        {
            if (args.Source.Id != this.Unit.Id)
            {
                return;
            }

            var name = args.SpellData.Name;

            if (Orbwalking.IsAutoAttackReset(name))
            {
                this.attackT = 0;
            }

            if (Orbwalking.IsAutoAttack(name))
            {
                this.attackT = Game.Time - PingOffset();
            }
        }

        private float attackT;

        private float PingOffset() => Game.Ping / 2000f;

        public bool CanAttack()
        {
            var attackSetting = Menu.GetMenu("speed")["attack"].GetValue<float>();

            return Game.Time + PingOffset() + attackSetting >= this.attackT + this.Unit.AttackDelay;
        }

        public bool CanMove()
        {
            var time = Game.Time + PingOffset();
            var attackCastDelay = this.Unit.AttackCastDelay;

            var logicSelected = this.Menu.GetMenu("speed")["logic"].GetValue<int>();

            if (logicSelected != 0)
            {
                if (time >= this.attackT + attackCastDelay)
                {
                    return true;
                }

                if (logicSelected == 2)
                {
                    return false;
                }
            }

            var cancelSetting = Menu.GetMenu("speed")["move"].GetValue<float>();

            var attackSpeedMod = this.Unit.AttackSpeedMod / 10f;
            var attackSpeed = 0.1f / this.Unit.AttackDelay;
            var percentAttackSpeedMod = this.Unit.PercentAttackSpeedMod / 10f;

            //Console.WriteLine("attackSpeedMod: " + attackSpeedMod);
            //Console.WriteLine("attackSpeed: " + attackSpeed);
            //Console.WriteLine("percentAttackSpeedMod: " + percentAttackSpeedMod);

            var magic = (attackSpeed + attackSpeedMod) * percentAttackSpeedMod * attackSpeed * 2f + percentAttackSpeedMod;

            return time + cancelSetting + magic >= this.attackT + attackCastDelay * (1f - attackSpeedMod);
        }

        private void OnUpdate(EventArgs obj)
        {
            var mode = Mode.Current;

            if (mode.IsNone)
            {
                return;
            }

            if (CanAttack())
            {
                var target = this.GetOrbwalkingTarget(mode);

                if (target != null && ProcessBeforeAttack(target) && Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                {
                    attackT = Game.Time + this.Unit.AttackDelay;
                }
            }

            var orderSetting = Menu.GetMenu("speed")["orderTime"].GetValue<float>();

            if (CanMove() && this.attackT <= Game.Time + orderSetting)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.Cursor);
            }
        }

        private IAttackable GetOrbwalkingTarget(Mode mode)
        {
            return ObjectManager.Get<IAttackable>().Where(t => t.IsValidTarget() && t.Distance(ObjectManager.Player) <= Orbwalking.GetAutoAttackRange(ObjectManager.Player, t)).OrderByDescending(h => h is IHero).FirstOrDefault();
        }
    }
}