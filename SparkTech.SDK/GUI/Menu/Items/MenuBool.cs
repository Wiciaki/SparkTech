namespace SparkTech.SDK.GUI.Menu.Items
{
    using System.Drawing;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Rendering;

    using Color = SharpDX.Color;

    public class MenuBool : MenuValue, IMenuValue<bool>
    {
        public MenuBool(string id, bool defaultValue) : base(id, defaultValue)
        {

        }

        private bool value;

        private Size buttonSize;

        protected internal override void OnEndScene(Point point, int groupWidth)
        {
            base.OnEndScene(point, groupWidth);

            point.X += groupWidth - this.buttonSize.Width;

            Theme.DrawBox(this.Value ? Color.Green : Color.Red, point, this.buttonSize);
        }

        protected override void UpdateTextBasedSize()
        {
            this.buttonSize = new Size(this.TextSize.Height, this.TextSize.Height);
        }

        #region Public Properties

        public bool Value
        {
            get => this.value;
            set
            {
                if (this.value == value)
                {
                    return;
                }

                this.value = value;

                this.OnPropertyChanged("IsActive");
            }
        }

        #endregion

        protected internal override void OnWndProc(Point point, int groupWidth, WndProcEventArgs args)
        {
            point.X += groupWidth - this.buttonSize.Width;

            this.Value ^= point.ToRectangle(this.buttonSize).IsCursorInside() && args.Message.IsLeftClick();
        }

        #region Properties

        protected override JToken Token
        {
            get => this.value;
            set => this.value = value.Value<bool>();
        }

        #endregion
    }
}