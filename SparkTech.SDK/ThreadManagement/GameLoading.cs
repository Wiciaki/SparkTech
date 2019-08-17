namespace SparkTech.SDK.TickOperations
{
    using System;
    using System.Collections.Generic;

    using SparkTech.SDK.Util;
    /*
    public static class GameLoading
    {
        #region Static Fields

        private static readonly List<Action> Callbacks = new List<Action>();

        #endregion

        #region Constructors and Destructors

        static GameLoading()
        {
            switch (Game.State)
            {
                case GameState.Running:
                case GameState.Paused:
                case GameState.Spawning:
                case GameState.Finished:
                    GameLoaded();
                    break;
                default:
                    void OnLoad(EntropyEventArgs args)
                    {
                        Game.OnLoad -= OnLoad;
                        GameLoaded();
                    }

                    Game.OnLoad += OnLoad;
                    break;
            }

            void GameLoaded()
            {
                IsLoaded = true;

                Callbacks.ForEach(Execute);
                Callbacks.Clear();
                Callbacks.TrimExcess();
            }
        }

        #endregion

        #region Public Events

        public static event Action OnLoad
        {
            add
            {
                if (IsLoaded)
                {
                    Execute(value);
                }
                else
                {
                    Callbacks.Add(value);
                }
            }
            remove => Callbacks.Remove(value);
        }

        #endregion

        private static void Execute(Action action)
        {
            action.TryExecute("Couldn't invoke a \"OnGameEntered\" callback.");
        }

        #region Public Properties

        public static bool IsLoaded { get; private set; }

        #endregion
    }*/
}