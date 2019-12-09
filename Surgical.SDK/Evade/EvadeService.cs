namespace Surgical.SDK.Evade
{
    using Surgical.SDK.Modules;

    public static class EvadeService
    {
        public static readonly Picker<IEvade> Picker = new Picker<IEvade>(new Evade());
    }
}