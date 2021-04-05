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
        public override Menu Menu { get; } = new Menu("spark")
        {
            new Menu("drawings")
            {
                new MenuColorBool("playerRange", Color.DodgerBlue, true),
                GetEnemyMenu(),
                new MenuColorBool("minions", Color.Silver, true),
                new MenuColorBool("playerHold", Color.DodgerBlue, false),
                new MenuColorBool("targetMinion", Color.Magenta, false)
            },
            new Menu("speed")
            {
                new MenuFloat("attack", 0f, 0.3f, 0.04f),
                new MenuFloat("move", 0f, 0.6f, 0.04f)
            },
            new MenuBool("Use missile checks for more speed sometimes", true),
            new MenuBool("underTurretFarming", true),
            new MenuBool("meleeMagnet", false)
        };

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

        private void Render_OnDraw()
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

        public override void Start()
        {
            if (!Platform.HasCoreAPI)
                return;

            Game.OnUpdate += this.Game_OnUpdate;
            Render.OnDraw += this.Render_OnDraw;
            EntityEvents.OnDoCast += this.EntityEvents_OnDoCast;
            EntityEvents.OnProcessSpellCast += this.EntityEvents_OnProcessSpellCast;
            EntityEvents.OnSpellbookStopCast += this.EntityEvents_OnSpellbookStopCast;
        }

        private void EntityEvents_OnSpellbookStopCast(StopCastEventArgs args)
        {
            if (args.Source?.Owner == null || !args.Source.Owner.IsMe() || !args.DestroyMissile || !args.KeepAnimationPlaying)
                return;

            attackT = 0;
        }

        private void EntityEvents_OnProcessSpellCast(ProcessSpellCastEventArgs args)
        {
            if (!args.Source.IsMe())
                return;

            var name = args.SpellData.Name;

            if (Orbwalking.IsAutoAttackReset(name))
                attackT = 0;

            if (Orbwalking.IsAutoAttack(name))
            {
                // ?
            }
        }

        private void EntityEvents_OnDoCast(ProcessSpellCastEventArgs args)
        {
            if (!args.Source.IsMe())
            {
                return;
            }

            var name = args.SpellData.Name;

            if (Orbwalking.IsAutoAttackReset(name))
            {
                attackT = 0;
            }

            if (Orbwalking.IsAutoAttack(name))
            {
                ProcessAfterAttack((IAttackable)args.Target);
                attackT = Game.Time - (Game.Ping / 2000f);
            }
        }

        public override void Pause()
        {
            if (!Platform.HasCoreAPI)
                return;

            Game.OnUpdate -= this.Game_OnUpdate;
            Render.OnDraw -= this.Render_OnDraw;
        }

        private float attackT;

        public bool CanAttack()
        {
            return Game.Time + (Game.Ping / 2000f) + Menu.GetMenu("speed")["attack"].GetValue<float>() >= this.attackT + ObjectManager.Player.AttackDelay;
        }

        public bool CanMove()
        {
            return Game.Time + (Game.Ping / 2000f) + Menu.GetMenu("speed")["move"].GetValue<float>() >= this.attackT + ObjectManager.Player.AttackCastDelay;
        }

        private void Game_OnUpdate(EventArgs obj)
        {
            if (Mode.NoneMode())
            {
                return;
            }

            if (Mode.Current.ChampsAutoAttack)
            {
                if (CanAttack())
                {
                    var target = this.GetOrbwalkingTarget();

                    if (target != null && ProcessBeforeAttack(target) && Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                    {
                        attackT = Game.Time + ObjectManager.Player.AttackDelay;
                    }
                }
            }

            if (CanMove())
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.Cursor);
            }
        }

        private IAttackable GetOrbwalkingTarget()
        {
            return ObjectManager.Get<IAttackable>().Where(t => t.IsValidTarget() && t.Distance(ObjectManager.Player) <= Orbwalking.GetAutoAttackRange(ObjectManager.Player, t)).OrderByDescending(h => h is IHero).FirstOrDefault();
        }
    }
}