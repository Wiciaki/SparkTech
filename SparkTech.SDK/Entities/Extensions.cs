namespace SparkTech.SDK.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using SharpDX;

    using SparkTech.SDK.Rendering;

    public static class Extensions
    {
        public static float HealthPercent(this IHero hero)
        {
            return hero.Health() / hero.MaxHealth() * 100f;
        }

        public static float ManaPercent(this IHero hero)
        {
            return hero.Mana() / hero.MaxMana() * 100f;
        }

        public static void DrawCircle(this IGameObject o, Color color, float radius)
        {
            Circle.Draw(color, radius, o.Position());
        }

        #region Static Fields

        private static readonly Regex LaneMinionRegex = new Regex(
            "(?:SRU|HA)_(?:Chaos|Order)Minion(.*)",
            RegexOptions.Compiled | RegexOptions.Singleline);

        #endregion

        #region Public Methods and Operators

        public static MinionType GetMinionType(this IMinion minion)
        {
            if (minion == null)
            {
                return MinionType.Unknown;
            }

            var baseSkinName = minion.BaseSkinName();
            var match = LaneMinionRegex.Match(baseSkinName);

            if (match.Success)
            {
                return match.Groups[1].Value switch
                {
                    "Melee" => MinionType.Melee,
                    "Ranged" => MinionType.Caster,
                    "Siege" => MinionType.Siege,
                    "Super" => MinionType.Super,
                    _ => throw new ArgumentOutOfRangeException(nameof(minion))
                };
            }

            if (baseSkinName.StartsWith("SRU_Plant", StringComparison.CurrentCultureIgnoreCase))
            {
                return MinionType.Plant;
            }

            baseSkinName = baseSkinName.ToLower();

            if (baseSkinName.Contains("ward") || baseSkinName.Contains("trinket"))
            {
                return MinionType.Ward;
            }

            var name = minion.Name();

            if (name.StartsWith("SRU_", StringComparison.OrdinalIgnoreCase))
            {
                return name.EndsWith("Mini") ? MinionType.JungleMini : MinionType.Jungle;
            }

            // other ???
            return MinionType.Summoned; // trundle wall, Jayce gate, heimer turret, malzahar voidling
        }

        public static bool IsJungle(this IMinion minion)
        {
            return minion.GetMinionType().IsJungle();
        }

        public static bool IsJungle(this MinionType type)
        {
            return type == MinionType.Jungle || type == MinionType.JungleMini;
        }

        public static bool IsJungleBuff(this IMinion minion)
        {
            switch (minion.CharName())
            {
                case "SRU_Blue":
                case "SRU_Red":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsMinion(this IMinion minion)
        {
            return minion.GetMinionType().IsMinion();
        }

        public static bool IsMinion(this MinionType type)
        {
            switch (type)
            {
                case MinionType.Melee:
                case MinionType.Caster:
                case MinionType.Siege:
                case MinionType.Super:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        private static readonly EntityComparer<IGameObject> GameObjectComparer = new EntityComparer<IGameObject>();

        public static bool Compare(this IGameObject left, IGameObject right)
        {
            return GameObjectComparer.Equals(left, right);
        }
        /*
        public static bool HasItem(this IEnumerable<InventorySlot> inventorySlot, params uint[] itemIds)
        {
            return inventorySlot.Any(i => itemIds.Contains(i.ItemID));
        }

        public static bool HasItem(this AIHeroClient target, params uint[] itemIds)
        {
            return target.InventorySlots.HasItem(itemIds);
        }

        public static bool IsMovementImpaired(this AIBaseClient unit)
        {
            return unit.BuffManager.Buffs.Exists(Buffs.Extensions.IsMovementImpairing);
        }

        public static bool IsValidTarget(this AttackableUnit t, bool mustBeEnemy = true)
        {
            /*

            if (t.Name == "WardCorpse")
            {
                return false;
            }

            

            return t != null && t.IsVisible && !t.IsDead && t.IsTargetable && !t.IsInvulnerable && (!mustBeEnemy || t.IsEnemy());
        }

        //TODO make this when Orientation is fixed
        public static bool IsFacingUnit(this AIBaseClient source, AIBaseClient target)
        {
            //TODO Server pos
            //return source.Orientation.To2D().Perpendicular().AngleBetween((target.Position - source.Postion).To2D()) < 90;
            return false;
        }

        public static float TotalShieldHealth(this AIBaseClient unit)
        {
            return unit.HP + unit.PhysicalShield + unit.MagicalShield;
        }

        public static float TotalPhysicalShieldHealth(this AIBaseClient unit)
        {
            return unit.HP + unit.PhysicalShield;
        }

        public static float TotalMagicalShieldHealth(this AIBaseClient unit)
        {
            return unit.HP + unit.MagicalShield;
        }*/
    }
}