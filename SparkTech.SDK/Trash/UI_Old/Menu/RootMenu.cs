//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  RootMenu.cs is a part of SparkTech
//
//  SparkTech is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  SparkTech is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  You should have received a copy of the GNU General Public License
//  along with SparkTech. If not, see <http://www.gnu.org/licenses/>.
//
//  -------------------------------------------------------------------

namespace SparkTech.SDK.UI.Menu
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SharpDX.Direct3D9;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Properties;
    using SparkTech.SDK.Util;

    using Color = SharpDX.Color;
    using Point = System.Drawing.Point;
    using Picture = SparkTech.SDK.Rendering.Picture;

    /// <summary>
    ///     The root menu class, which manages the menu structure.
    /// </summary>
    public class RootMenu : Menu
    {
        private static readonly List<RootMenu> Roots;

        private static Point menuStart, lastStart;

        private static Size dragSize;

        private static readonly SharpDX.Direct3D9.Texture ButtonTexture;

        private static readonly Size ArrowSize;

        private static readonly string MenuPositionFilePath;

        #region Constructors and Destructors

        static RootMenu()
        {
            menuStart = new Point(40, 40);

            var arrow = Resources.MenuMoveArrow;

            ButtonTexture = arrow.ToTexture();

            ArrowSize = arrow.Size;

            Roots = new List<RootMenu>();

            MenuPositionFilePath = Path.Combine(SaveDirectory, "MenuPosition");

            if (File.Exists(MenuPositionFilePath))
            {
                try
                {
                    var arr = JArray.Parse(File.ReadAllText(MenuPositionFilePath));

                    menuStart.X = arr[0].Value<int>();
                    menuStart.Y = arr[1].Value<int>();
                }
                catch (Exception ex)
                {
                    ex.LogException("Couldn't read the saved menu position.");
                }
            }

            lastStart = menuStart;

            VisibilityStateChanged += MenuVisibilityStateChanged;
            GameEvents.OnWndProc += GameOnWndProc;
        }

        private static async void MenuVisibilityStateChanged()
        {
            if (IsOpen)
            {
                Renderer.OnEndScene += RendererOnEndScene;
                return;
            }

            Renderer.OnEndScene -= RendererOnEndScene;

            Roots.ForEach(Save);

            if (lastStart == menuStart)
            {
                return;
            }

            lastStart = menuStart;

            Logging.Log("Saving the updated menu position...");

            await EntropySetup.SaveToFileAsync(MenuPositionFilePath, new JArray { menuStart.X, menuStart.Y });

            Logging.Log("Saved the menu position!");
        }

        private JToken lastSavedValue;

        private static async void Save(RootMenu root)
        {
            if (!root.ShouldSave())
            {
                return;
            }

            Logging.Log($"Saving the updated values for \"{root.Id}\"...");

            var token = root.GetToken();

            if (token == null)
            {
                Logging.Log("All values are default, nothing to save...");
                root.lastSavedValue = null;
                File.Delete(root.targetFile);
                return;
            }

            if (JToken.DeepEquals(root.lastSavedValue, token))
            {
                Logging.Log("Nothing needs saving, aborting...");
                return;
            }

            root.lastSavedValue = token;

            await EntropySetup.SaveToFileAsync(root.targetFile, token);

            Logging.Log($"Saving completed for \"{root.Id}\"!");
        }

        private static void GameOnWndProc(GameWndProcEventArgs args)
        {
            if (!IsOpen)
            {
                return;
            }

            var visible = Roots.FindAll(r => r.IsVisible);

            if (visible.Count == 0)
            {
                return;
            }

            SetCursorPos();

            MoveMenu(args.Message);

            var size = new Size(visible.Max(r => r.Size.Width), 0);

            var point = menuStart;
            point.X += ArrowSize.Width;
            point.Y += ArrowSize.Height;

            var check = args.Message.IsLeftClick();

            visible.ForEach(WndProc);

            void WndProc(RootMenu root)
            {
                size.Height = root.Size.Height;

                root.OnWndProc(point, size, args);

                if (check && Mouse.IsInside(point, size))
                {
                    check = false;

                    Roots.ForEach(r => r.IsHighlighted = r.Id == root.Id && !r.IsHighlighted);
                }

                point.Y += root.Size.Height + Theme.ItemDistance.Height;
            }
        }

        private static bool dragging;

        internal static bool IsMovable;

        private static void MoveMenu(WindowMessage message)
        {
            if (message.IsLeftClick())
            {
                dragging = IsMovable && Mouse.IsInside(menuStart, ArrowSize);

                dragSize = new Size(menuStart.X - Mouse.X, menuStart.Y - Mouse.Y);
            }
            else if (message == WindowMessage.LBUTTONUP)
            {
                dragging = false;
            }
        }

        private static void SetCursorPos()
        {
            Mouse = SDK.Game.ScreenCursorPos.ToPoint();
        }

        private static void RendererOnEndScene(EntropyEventArgs args)
        {
            var visible = Roots.FindAll(r => r.IsVisible);

            if (visible.Count == 0)
            {
                return;
            }

            if (dragging)
            {
                SetCursorPos();

                DragMenu();
            }

            var size = new Size(visible.Max(r => r.Size.Width), 0);

            var point = menuStart;

            if (IsMovable)
            {
                Picture.Render(point.ToVector2(), ButtonTexture, Color.White);
            }

            point += ArrowSize;

            visible.ForEach(EndScene);

            void EndScene(RootMenu root)
            {
                size.Height = root.Size.Height;

                root.OnEndScene(point, size);
                root.OnDrawOverlay(point, size);

                point.Y += root.Size.Height + Theme.ItemDistance.Height;
            }
        }

        private static void DragMenu()
        {
            var x = Mouse.X + dragSize.Width;

            if (x <= 0)
            {
                x = 0;
            }

            var r = (int)Renderer.ScreenResolutionX - ArrowSize.Width;

            if (x > r)
            {
                x = r;
            }

            var y = Mouse.Y + dragSize.Height;

            if (y <= 0)
            {
                y = 0;
            }

            r = (int)Renderer.ScreenResolutionY - ArrowSize.Height;

            if (y > r)
            {
                y = r;
            }

            menuStart = new Point(x, y);
        }

        private readonly string targetFile;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RootMenu" /> class.
        /// </summary>
        /// <param name="id">
        ///     The unique identifier of the <see cref="MenuComponent" />
        /// </param>
        public RootMenu(string id) : base(id)
        {
            this.targetFile = Path.Combine(SaveDirectory, id + ".json");

            if (File.Exists(this.targetFile))
            {
                using (var sr = new StreamReader(this.targetFile))
                using (var reader = new JsonTextReader(sr))
                {
                    try
                    {
                        this.Settings = (JObject)JToken.ReadFrom(reader);
                    }
                    catch (Exception ex)
                    {
                        ex.LogException($"Failed to load JSON file for \"{this.Id}\"");

                        this.Settings = new JObject();
                    }
                }
            }
            else
            {
                this.Settings = new JObject();
            }

            Roots.Add(this);

            this.UpdateSize();
        }

        public void Translate(JObject token)
        {
            this.UpdateTranslations(token);
        }

        protected internal sealed override void UpdateSize() => base.UpdateSize();

        public async Task SaveTranslationTemplate(string targetPath)
        {
            await EntropySetup.SaveToFileAsync(targetPath, this.GetRequiredTranslations());
        }

        #endregion
    }
}