namespace SparkTech.SDK.Utilities
{
    using SparkTech.SDK.GUI.Menu;
    using System;

    using SparkTech.SDK.Entities;

    public class AutoLvlUp : IUtility
    {
        public void Start()
        {
            if (Platform.HasCoreAPI)
            {
                Game.OnUpdate += OnUpdate;
            }
        }

        public void Pause()
        {
            if (Platform.HasCoreAPI)
            {
                Game.OnUpdate -= OnUpdate;
            }
        }

        private static void OnUpdate(EventArgs _)
        {
            if (ObjectManager.Player.SpellTrainingPoints == 0)
            {
                return;
            }


        }

        public Menu Menu { get; }

        public AutoLvlUp()
        {
            this.Menu = new Menu("leveler");
        }
    }
}