﻿namespace SparkTech.SDK.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using SharpDX;

    using SparkTech.SDK.League;

    public static class Extensions
    {
        public static bool IsMe(this IGameObject o)
        {
            return o.Id == ObjectManager.Player.Id;
        }

        public static bool IsAlly(this IGameObject o)
        {
            return o.Team == ObjectManager.Player.Team;
        }

        public static bool IsEnemy(this IGameObject o)
        {
            return !o.IsAlly();
        }

        public static bool IsDummy(this IHero hero)
        {
            return hero.CharName == "PracticeTool_TargetDummy";
        }

        public static float HealthPercent(this IHero hero)
        {
            return hero.Health / hero.MaxHealth * 100f;
        }

        public static float ManaPercent(this IHero hero)
        {
            return hero.Mana / hero.MaxMana * 100f;
        }

        #region Static Fields

        private static readonly Regex LaneMinionRegex = new Regex(
            "(?:SRU|HA)_(?:Chaos|Order)Minion(.*)",
            RegexOptions.Compiled | RegexOptions.Singleline);

        #endregion

        #region Public Methods and Operators

        public static MinionType GetMinionType(this IMinion minion)
        {
            var baseSkinName = minion.SkinName;
            var match = LaneMinionRegex.Match(baseSkinName);

            if (match.Success)
            {
                switch (match.Groups[1].Value)
                {
                    case "Melee":
                        return MinionType.Melee;
                    case "Ranged":
                        return MinionType.Caster;
                    case "Siege":
                        return MinionType.Siege;
                    case "Super":
                        return MinionType.Super;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(minion));
                }
            }

            if (baseSkinName.StartsWith("SRU_Plant", StringComparison.InvariantCultureIgnoreCase))
            {
                return MinionType.Plant;
            }

            baseSkinName = baseSkinName.ToLower();

            if (baseSkinName.Contains("ward") || baseSkinName.Contains("trinket"))
            {
                return MinionType.Ward;
            }

            var name = minion.Name;

            if (name.StartsWith("SRU_", StringComparison.InvariantCultureIgnoreCase))
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
            switch (minion.CharName)
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

        private static readonly IEqualityComparer<IGameObject> GameObjectComparer = new EntityComparer<IGameObject>();

        public static bool Compare(this IGameObject left, IGameObject right)
        {
            return right != null && GameObjectComparer.Equals(left, right);
        }

        public static bool Compare(this IGameObject left, int rightId)
        {
            return left.Id == rightId;
        }

        public static bool IsMovementImpairing(this IBuff buff)
        {
            return buff.Type.IsMovementImpairing();
        }

        public static bool IsMovementImpairing(this BuffType buffType)
        {
            switch (buffType)
            {
                case BuffType.Flee:
                case BuffType.Charm:
                case BuffType.Slow:
                case BuffType.Snare:
                case BuffType.Stun:
                case BuffType.Taunt:
                    return true;
                default:
                    return false;
            }
        }

        public static bool PreventsCasting(this IBuff buff)
        {
            return buff.Type.PreventsCasting();
        }

        public static bool PreventsCasting(this BuffType buffType) // todo review?
        {
            switch (buffType)
            {
                case BuffType.Silence:
                case BuffType.Charm:
                case BuffType.Taunt:
                case BuffType.Knockup:
                case BuffType.Flee:
                case BuffType.Suppression:
                case BuffType.Stun:
                case BuffType.Polymorph:
                case BuffType.Disarm:
                case BuffType.Knockback:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsValid(this IBuff buff)
        {
            if (buff == null)
            {
                return false;
            }

            var t = Game.Time;

            return t >= buff.StartTime && t < buff.EndTime;
        }

        public static float TimeLeft(this IBuff buff)
        {
            return Math.Max(0, buff.EndTime - Game.Time);
        }

        public static bool IsValidTarget(this IAttackable t, bool mustBeEnemy = true)
        {
            if (t == null || t.Name == "WardCorpse")
            {
                return false;
            }

            if (!t.IsVisible || t.IsDead || !t.IsTargetable || t.IsInvulnerable || mustBeEnemy && !t.IsEnemy())
            {
                return false;
            }

            if (t is IUnit unit && !unit.IsHPBarRendered)
            {
                return false;
            }

            return true;
        }

        /*
        private const float TurretRange = 950f;

        public static bool IsUnderTurret(this Vector3 position, ObjectTeam team = ObjectTeam.Enemy)
        {
            return ObjectManager.Get<ITurret>().Any(t => t.Team() == team && position.Distance(t.Position) < TurretRange + t.BoundingRadius);
        }

        public static bool IsUnderTurret(this IAIBase target, GameObjectTeam team = GameObjectTeam.)
        {
            return IsUnderTurret(target.Position, team);
        }
        */

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