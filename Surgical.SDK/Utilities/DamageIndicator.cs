namespace Surgical.SDK.Utilities
{
    using Surgical.SDK.GUI.Menu;

    public class DamageIndicator : IUtility
    {
        public void Start()
        {

        }

        public void Pause()
        {

        }

        public Menu Menu { get; }

        public DamageIndicator()
        {
            this.Menu = new Menu("indicator");
        }
    }
}