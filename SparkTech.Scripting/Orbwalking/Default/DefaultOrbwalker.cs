﻿namespace SparkTech.Orbwalking.Default
{
    using System;
    using System.Collections.Generic;

    using SparkTech.UI.Menu;
    using SparkTech.UI.Menu.Values;
    using SparkTech.Utils;

    public class DefaultOrbwalker : IOrbwalker
    {
        ModuleMenu IEntropyModule.Menu => Menu;

        private static readonly ModuleMenu Menu;

        private static readonly Menu KeysMenu;

        static DefaultOrbwalker()
        {
            Modes = new List<Mode> { Mode.Combo, Mode.Lasthit, Mode.Mixed, Mode.Laneclear, Mode.Flee };

            KeysMenu = new Menu("Keys")
                       {
                           new MenuKeyBind("Combo", WindowMessageWParam.Space),
                           new MenuKeyBind("Lasthit", WindowMessageWParam.X),
                           new MenuKeyBind("Mixed", WindowMessageWParam.C),
                           new MenuKeyBind("Laneclear", WindowMessageWParam.V),
                           new MenuKeyBind("Flee", WindowMessageWParam.Z)
                       };

            Menu = new ModuleMenu("Orbwalker") { KeysMenu };
        }

        protected virtual AIBaseClient Unit => LocalPlayer.Instance;

        #region Constructors and Destructors

        public DefaultOrbwalker()
        {
            AIBaseClient.OnProcessBasicAttack += this.OnProcessBasicAttack;
            Renderer.OnEndScene += this.OnEndScene;
            Spellbook.OnStopCast += this.OnStopCast;
        }

        private void OnStopCast(SpellbookStopCastEventArgs args)
        {
            if (!this.Unit.Compare(args.Sender))
            {

            }
        }

        private void OnEndScene(EntropyEventArgs args)
        {

        }

        private void OnProcessBasicAttack(AIBaseClientCastEventArgs args)
        {
            if (args.Caster.IsMe())
            {
                this.LastAttackStartTime = args.ExecuteTime;

                this.IsAttacking = true;
            }
        }

        private static readonly List<Mode> Modes;

        public Mode GetMode()
        {
            return Modes.Find(n => KeysMenu[n.ToString()].GetValue<bool>());
        }

        #endregion

        #region Explicit Interface Methods

        void IEntropyModule.Release()
        {
            AIBaseClient.OnProcessBasicAttack -= this.OnProcessBasicAttack;
            Spellbook.OnStopCast -= this.OnStopCast;
            Renderer.OnEndScene -= this.OnEndScene;
        }

        #endregion

        public float LastAttackStartTime { get; private set; }

        public bool IsAttacking { get; private set; }

        public float LastOrderTime { get; private set; }

        public event Action<BeforeAttackEventArgs> BeforeAttack;

        public event Action<AttackableUnit> AfterAttack;
    }
}