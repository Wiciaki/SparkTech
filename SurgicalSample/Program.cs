namespace SurgicalSample
{
    using System;
    using System.Collections.Generic;

    using SharpDX;

    using Surgical.SDK;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.Logging;
    using Surgical.SDK.Modules;
    using Surgical.SDK.Rendering;

    // all classes inheriting from the empty IEntryPoint interface are having their empty constructor executed on load
    // same applies to all modules - IOrbwalker, ITargetSelector etc - as these interfaces inherit from IEntryPoint, too

    // ReSharper disable once UnusedMember.Global
    public sealed class Program : IEntryPoint
    {
        public Program()
        {
            // this is the entry point for the script
            Log.Info("Hello, World! (hello, logs...?)");

            // creates the menu
            this.menu = new Menu("SampleMenu")
                        {
                            new Menu("FirstSubmenu")
                            {
                                new MenuBool("ThisIsAwesome", true)
                            },
                            new MenuInt("ThisIsWhatSliderLooksLike", 0, 100, 50),
                            new MenuInt("SlidersCanBeReverseToo", 500, 100, 350),
                            new MenuList("HaveFun")
                            {
                                Options = new List<string> { "Exploring", "All", "The", "SDK", "Possibilities" }
                            },
                            new MenuColorBool("TextColor", Color.White, false)
                        };

            // put JObject as a Build(...) parameter
            // this will enable multilanguage, too!
            this.menu.GetMenu("FirstSubmenu").Get<MenuText>("ThisIsAwesome").HelpText = "This is easier to set if you use json to provide translations";

            // adds our menu to the main menu
            Menu.Build(this.menu);

            // OnStart will not work if we're using GUI Editor or similiar
            if (Platform.HasCoreAPI)
            {
                // will execute the code after loading screen
                // or immediately, if the game has already started
                Game.OnStart += this.OnStart;
            }
            else
            {
                Render.OnDraw += this.OnDraw;
            }
        }

        // the menu instance. Use it to check user settings.
        private readonly Menu menu;

        private void OnStart(EventArgs obj)
        {
            // subscribe to OnUpdate, which executes every tick, which is... well... very often!
            Game.OnUpdate += this.OnUpdate;

            // if we subscribe to OnDraw here, we prevent from drawing on loading screen
            // ... and errors, since game stuff cant be used yet
            Render.OnDraw += this.OnDraw;
        }

        private void OnDraw()
        {
            // don't draw if it's disabled
            if (!this.menu["TextColor"].GetValue<bool>())
            {
                return;
            }

            const string AwesomeText = "Hell yeah, this sample script is A W E S O M E";
            var color = this.menu["TextColor"].GetValue<Color>();
            var drawPoint = (Point)UserInput.CursorPosition;
            drawPoint.Y += 20;

            Text.Draw(AwesomeText, color, drawPoint);
        }

        private void OnUpdate(EventArgs obj)
        {
            // this is how to get a get a menu value
            var isEnabled = this.menu.GetMenu("FirstSubmenu")["ThisIsAwesome"].GetValue<bool>();
        }
    }
}