namespace SparkTech.Utils
{
    public static class Hero
    {
        public static float MPPercent(this AIHeroClient hero)
        {
            return hero.MP / hero.MaxMP * 100;
        }
    }
}