namespace SparkTech.SDK.UI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using SparkTech.SDK.Modules;
    using SparkTech.SDK.Platform;
    using SparkTech.SDK.UI.Default;
    using SparkTech.SDK.Util;

    public static class Theme
    {
        #region Static Fields

        private static readonly IModulePicker<ITheme> Picker;

        #endregion

        #region Constructors and Destructors

        static Theme()
        {
            Picker = SdkSetup.CreatePicker<ITheme, GrayTheme>();
            Picker.Add<PurpleTheme>("Purple");
            Picker.ModuleSelected += Proxy.InvokeThemeSelected;
        }

        public static void Add<T>(string moduleName) where T : ITheme, new()
        {
            Picker.Add<T>(moduleName);
        }

        public static Color BackgroundColor => Picker.Current.BackgroundColor;

        public static Color FontColor => Picker.Current.FontColor;

        public static Size ItemDistance => Picker.Current.ItemDistance;

        public static Size MeasureSize(string text) => Picker.Current.MeasureSize(text);

        public static void Draw(DrawData data) => Picker.Current.Draw(data);

        internal static class Proxy
        {
            private static bool loaded;

            private static readonly List<Action> Callbacks = new List<Action>();

            internal static event Action ThemeSelected
            {
                add
                {
                    Callbacks.Add(value);

                    if (loaded)
                    {
                        value.TryExecute("Couldn't invoke a newly added ThemeSelected listener!");
                    }
                }
                remove => Callbacks.Remove(value);
            }

            internal static void InvokeThemeSelected()
            {
                loaded = true;

                Callbacks.ForEach(a => a.TryExecute("Couldn't invoke a ThemeSelected listener!"));
            }
        }

        #endregion
    }
}