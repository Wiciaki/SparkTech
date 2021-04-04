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
                foreach (var hero in ObjectManager.Get<IHero>().Where(h => h.IsEnemy()))
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

            LastAutoAttackStartT = 0;
        }

        private void EntityEvents_OnProcessSpellCast(ProcessSpellCastEventArgs args)
        {
            if (!args.Source.IsMe())
                return;

            var name = args.SpellData.Name;

            if (Orbwalking.IsAutoAttackReset(name))
                LastAutoAttackStartT = 0;

            if (Orbwalking.IsAutoAttack(name))
            {
                missileLaunched = true;
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
                LastAutoAttackStartT = 0;
            }

            if (Orbwalking.IsAutoAttack(name))
            {
                ProcessAfterAttack((IAttackable)args.Target);
                LastAutoAttackStartT = Game.Time - (Game.Ping / 2000f);
                missileLaunched = false;
            }
        }

        public override void Pause()
        {
            if (!Platform.HasCoreAPI)
                return;

            Game.OnUpdate -= this.Game_OnUpdate;
            Render.OnDraw -= this.Render_OnDraw;
        }

        private bool missileLaunched;

        private float LastAutoAttackStartT;

        public bool CanAttack()
        {
            return Game.Time + ((Game.Ping + 50) / 2000f) >= LastAutoAttackStartT + ObjectManager.Player.AttackDelay;
        }

        public bool CanMove()
        {
            return missileLaunched || Game.Time + ((Game.Ping + 50) / 2000f) >= LastAutoAttackStartT + ObjectManager.Player.AttackCastDelay;
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

                    if (target != null && ProcessBeforeAttack(target))
                    {
                        missileLaunched = false;
                        LastAutoAttackStartT = float.MaxValue;
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        return;
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
            return ObjectManager.Get<IAttackable>().FirstOrDefault(t => t.IsValidTarget() && t.Distance(ObjectManager.Player) <= Orbwalking.GetAutoAttackRange(ObjectManager.Player, t));
        }
    }
}