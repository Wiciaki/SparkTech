namespace SparkTech.Database
{
    using System;

    using SparkTech.Database.Default;

    public static class Database
    {
        private static readonly IModulePicker<IDatabase> Picker = EntropySetup.CreatePicker<IDatabase, DefaultDatabase>();

        public static Version Version => Picker.Current.Version;

        public static void Add<T>(string moduleName) where T : IDatabase, new()
        {
            Picker.Add<T>(moduleName);
        }

        public static void RegisterDataObtainer(Action obtainer)
        {
            Picker.ModuleSelected += obtainer;

            obtainer();
        }
    }
}