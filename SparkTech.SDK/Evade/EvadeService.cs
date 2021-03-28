namespace SparkTech.SDK.Evade
{
    using SparkTech.SDK.Modules;

    public static class EvadeService
    {
        public static readonly Picker<IEvade> Picker = new Picker<IEvade>(new Evade());
    }
}