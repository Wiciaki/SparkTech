namespace SparkTech.Ensoul
{
    using SDK;
    using SDK.Licensing;

    using SparkTech.SDK.DamageLibrary;

    class Program
    {
        static void Main(string[] _)
        {
            var platform = new Platform("EnsoulSharp")
            {
                AuthResult = AuthResult.GetLifetime(),
                WatermarkOffset = 13,

                RenderAPI = new RenderAPI(),
                UserInputAPI = new UserInputAPI(),
                CoreAPI = new CoreAPI()
            };

            platform.Load();

            DamageLibraryService.Picker.Add(new Ports.EnsoulDamageLibrary());
            DamageLibraryService.Picker.OnModuleSelected += args => args.Block();
        }
    }
}