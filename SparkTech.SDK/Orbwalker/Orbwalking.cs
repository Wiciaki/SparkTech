namespace SparkTech.SDK.Orbwalker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.Properties;

    public static class Orbwalking
    {
        public static readonly IList<string> AttackResets;

        public static readonly IList<string> Attacks;

        public static readonly IList<string> NoAttacks;

        static Orbwalking()
        {
            string[] Parse(string resource)
            {
                return JArray.Parse(resource).Values<string>().ToArray();
            }

            AttackResets = Parse(Resources.AttackResets);
            Attacks = Parse(Resources.Attacks);
            NoAttacks = Parse(Resources.NoAttacks);
        }

        public static bool IsAutoAttack(string name)
        {
            name = name.ToLower();

            return name.Contains("attack") && !NoAttacks.Contains(name) || Attacks.Contains(name);
        }

        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        public static bool IsMelee(this IUnit unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }

        public static float GetAutoAttackRange(IUnit source)
        {
            return source.AttackRange + source.BoundingRadius;
        }

        public static float GetAutoAttackRange(IUnit source, IAttackable target)
        {
            if (target is IUnit unit)
            {
                var champ = source.CharName;

                if (champ == "Caitlyn" && unit.HasBuff("caitlynyordletrapinternal"))
                {
                    return 1300f;
                }

                if (champ == "Aphelios" && source.HasBuff("aphelioscalibrumbonusrangebuff") && unit.HasBuff("aphelioscalibrumbonusrangedebuff"))
                {
                    return 1800f;
                }
            }

            return GetAutoAttackRange(source) + target.BoundingRadius;
        }
    }
}