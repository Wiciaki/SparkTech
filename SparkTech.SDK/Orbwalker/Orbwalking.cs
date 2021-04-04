namespace SparkTech.SDK.Orbwalker
{
    using System;
    using System.Collections.Generic;

    using SparkTech.SDK.Entities;

    public static class Orbwalking
    {
        public static readonly IList<string> AttackResets = new string[]
    {
      "asheq",
      "camilleq2",
      "camilleq",
      "dariusnoxiantacticsonh",
      "elisespiderw",
      "fiorae",
      "gravesmove",
      "garenq",
      "gangplankqwrapper",
      "illaoiw",
      "jaycehypercharge",
      "jaxempowertwo",
      "kaylee",
      "luciane",
      "leonashieldofdaybreakattack",
      "leonashieldofdaybreak",
      "mordekaisermaceofspades",
      "monkeykingdoubleattack",
      "meditate",
      "masochism",
      "netherblade",
      "nautiluspiercinggaze",
      "nasusq",
      "powerfist",
      "rengarqemp",
      "rengarq",
      "renektonpreexecute",
      "reksaiq",
      "settq",
      "sivirw",
      "shyvanadoubleattack",
      "sejuaninorthernwinds",
      "trundletrollsmash",
      "talonnoxiandiplomacy",
      "takedown",
      "vorpalspikes",
      "volibearq",
      "vie",
      "vaynetumble",
      "xinzhaoq",
      "xinzhaocombotarget",
      "yorickspectral",
      "apheliosinfernumq",
      "gravesautoattackrecoilcastedummy"
    };

        public static readonly IList<string> Attacks = new string[13]
        {
      "caitlynheadshotmissile",
      "itemtitanichydracleave",
      "itemtiamatcleave",
      "kennenmegaproc",
      "masteryidoublestrike",
      "quinnwenhanced",
      "renektonsuperexecute",
      "renektonexecute",
      "trundleq",
      "viktorqbuff",
      "xinzhaoqthrust1",
      "xinzhaoqthrust2",
      "xinzhaoqthrust3"
        };

        public static readonly IList<string> NoAttacks = new string[48]
        {
      "asheqattacknoonhit",
      "annietibbersbasicattack",
      "annietibbersbasicattack2",
      "bluecardattack",
      "dravenattackp_r",
      "dravenattackp_rc",
      "dravenattackp_rq",
      "dravenattackp_l",
      "dravenattackp_lc",
      "dravenattackp_lq",
      "elisespiderlingbasicattack",
      "gravesbasicattackspread",
      "gravesautoattackrecoil",
      "goldcardattack",
      "heimertyellowbasicattack",
      "heimertyellowbasicattack2",
      "heimertbluebasicattack",
      "heimerdingerwattack2",
      "heimerdingerwattack2ult",
      "ivernminionbasicattack2",
      "ivernminionbasicattack",
      "kindredwolfbasicattack",
      "monkeykingdoubleattack",
      "malzaharvoidlingbasicattack",
      "malzaharvoidlingbasicattack2",
      "malzaharvoidlingbasicattack3",
      "redcardattack",
      "shyvanadoubleattackdragon",
      "shyvanadoubleattack",
      "talonqdashattack",
      "talonqattack",
      "volleyattackwithsound",
      "volleyattack",
      "yorickghoulmeleebasicattack",
      "yorickghoulmeleebasicattack2",
      "yorickghoulmeleebasicattack3",
      "yorickbigghoulbasicattack",
      "zyraeplantattack",
      "zoebasicattackspecial1",
      "zoebasicattackspecial2",
      "zoebasicattackspecial3",
      "zoebasicattackspecial4",
      "apheliosseverumattackmis",
      "aphelioscrescendumattackmisin",
      "aphelioscrescendumattackmisout",
      "gravesautoattackrecoilcastedummy",
      "gravesautoattackrecoil",
      "gravesbasicattackspread"
        };

        public static bool IsAutoAttack(string name)
        {
            name = name.ToLower();

            return name.Contains("attack") && !NoAttacks.Contains(name) || Attacks.Contains(name);
        }

        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        public static float GetAutoAttackRange(IUnit source)
        {
            return source.AttackRange + source.BoundingRadius;
        }

        public static float GetAutoAttackRange(IUnit source, IAttackable target)
        {
            return GetAutoAttackRange(source) + target.BoundingRadius;
        }

        public static bool IsMelee(this IUnit unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }
    }
}