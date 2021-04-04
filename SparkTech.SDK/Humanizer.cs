namespace SparkTech.SDK
{
    using SparkTech.SDK.GUI.Menu;

    public static class Humanizer
    {
        internal static void Initialize(Menu menu)
        {
            menu.Add(new MenuBool("enable", true));
            menu.Add(new MenuInt("apm", 200, 600, 400));
        }
    }
}