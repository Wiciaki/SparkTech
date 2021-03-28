namespace SparkTech.Ensoul
{
    using SDK;
    using SDK.Licensing;

    class Program
    {
        static void Main(string[] args)
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
        }
    }
}