﻿namespace SparkTech.HealthPrediction.Default
{
    using System;

    using SparkTech.HealthPrediction.Default.Models;
    using SparkTech.TickOperations;
    using SparkTech.UI.Menu;
    using SparkTech.Utils;

    public class DefaultHealthPrediction : IHealthPredition
    {
        ModuleMenu IEntropyModule.Menu => null;

        #region Constructors and Destructors

        public DefaultHealthPrediction()
        {
            //Starting events
            AIBaseClient.OnProcessBasicAttack += this.AIBaseClient_OnProcessBasicAttack;
            AIBaseClient.OnFinishCast += this.AIBaseClient_OnFinishCast;
            //TODO use cache in the future
            GameObject.OnCreate += this.GameObject_OnCreate;
            GameObject.OnDelete += this.GameObject_OnDelete;

            Spellbook.OnStopCast += this.Spellbook_OnStopCast;

            GameTick.OnUpdate += this.GameTick_OnUpdate;
        }

        #endregion

        #region Public Methods and Operators

        public uint RangeToDetectAttacks { get; set; } = 3000;

        public float PredictHealth(AIBaseClient unit, float time)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        private void AIBaseClient_OnFinishCast(AIBaseClientCastEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void AIBaseClient_OnProcessBasicAttack(AIBaseClientCastEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void GameTick_OnUpdate(EntropyEventArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Caching created objects aka Missiles
        /// </summary>
        /// <param name="args"></param>
        private void GameObject_OnCreate(GameObjectCreateEventArgs args)
        {
            if (!(args.Sender is MissileClient mc))
            {
                return;
            }

            if (!mc.Target?.IsValid != true || mc.Distance(LocalPlayer.Instance) > this.RangeToDetectAttacks)
            {
                return;
            }

            //TODO fix this
            var attack = new RangedAttack((AIBaseClient)args.Sender, mc.Target, mc, 0);
            //Attacks.AddAttack(attack);
        }

        private void GameObject_OnDelete(GameObjectDeleteEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void Spellbook_OnStopCast(SpellbookStopCastEventArgs args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}