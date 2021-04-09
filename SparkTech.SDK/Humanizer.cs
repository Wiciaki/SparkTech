namespace SparkTech.SDK
{
    using System;

    using SparkTech.SDK.GUI.Menu;

    public class Humanizer
    {
        private static readonly Random Random = new Random();

        private static Menu menu;

        private float commandT;

        internal static void Initialize(Menu m)
        {
            m.Add(new MenuBool("enable", true));
            m.Add(new MenuInt("apm", 200, 600, 400));

            menu = m;
        }

        private static float GetDelay()
        {
            var apm = menu["apm"].GetValue<int>();
            var t = apm / 60000f;

            Console.WriteLine(t);

            return 0f;
        }

        public bool Execute(Func<bool> func)
        {
            var time = Game.Time;

            if (this.commandT > time && menu["enable"].GetValue<bool>() || !func())
            {
                return false;
            }

            this.commandT = time + GetDelay();
            return true;
        }
    }
}