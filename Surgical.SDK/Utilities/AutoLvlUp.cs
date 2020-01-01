namespace Surgical.SDK.Utilities
{
    using Surgical.SDK.GUI.Menu;

    public class AutoLvlUp : IUtility
    {
        public void Start()
        {
            if (Platform.HasCoreAPI)
            {
                return;
            }


        }

        public void Pause()
        {

        }

        public Menu Menu { get; }

        public AutoLvlUp()
        {
            this.Menu = new Menu("leveler");
        }
    }
}