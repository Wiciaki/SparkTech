namespace Surgical.SDK.Evade
{
    using Surgical.SDK.API;
    using Surgical.SDK.Modules;

    public static class Evade
    {
        public static readonly IModulePicker<IEvade> Picker = new SdkSetup.Picker<IEvade>(new SurgicalEvade());
    }
}