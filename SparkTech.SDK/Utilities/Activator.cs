namespace SparkTech.SDK.Utilities
{
    using SparkTech.SDK.GUI.Menu;

    public class Activator : IUtility
    {
        public void Start()
        {
            
        }

        public void Pause()
        {

        }

        public Menu Menu { get; }

        public Activator()
        {
            this.Menu = new Menu("activator");
        }
    }
}