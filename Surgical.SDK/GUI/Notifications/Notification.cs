namespace Surgical.SDK.GUI.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    using Surgical.SDK.Rendering;

    public class Notification
    {
        private static float decayTime;

        private static bool borders;

        internal static void SetBorders(bool bordersEnabled)
        {
            borders = bordersEnabled;
        }

        internal static void SetDecayTime(float time)
        {
            decayTime = time;
        }

        public readonly string Content;

        public readonly string? Header;

        public Notification(string content, string? header = null)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("No content provided", nameof(content));
            }

            this.Content = content;
            this.Header = header;

            this.UpdateSizes();
        }

        private static readonly List<NotificationEntry> Entries = new List<NotificationEntry>();

        public void Send(float duration)
        {
            Entries.Add(new NotificationEntry(this, duration));
        }

        public static void Send(string content, float duration)
        {
            Send(content, null, duration);
        }

        public static void Send(string content, string? header = null)
        {
            Send(content, header, 5f);
        }

        public static void Send(string content, string? header, float duration)
        {
            new Notification(content, header).Send(duration);
        }

        private Size2 headerSize, contentSize;

        private int Width => Math.Max(this.headerSize.Width, this.contentSize.Width);

        private void UpdateSizes()
        {
            var csize = Theme.MeasureText(this.Content);

            if (Menu.Menu.MinNotificationWidth > csize.Width)
            {
                csize.Width = Menu.Menu.MinNotificationWidth;
            }

            this.contentSize = csize;

            if (this.Header != null)
            {
                this.headerSize = Theme.MeasureText(this.Header);
            }
        }

        static Notification()
        {
            Render.OnEndScene += OnEndScene;
        }

        private static float GetTime()
        {
            return Environment.TickCount / 1000f;
        }

        private static void OnEndScene()
        {
            if (Entries.Count == 0)
            {
                return;
            }

            var point = new Point(1870, 100);
            var time = GetTime() - decayTime;
            var width = Entries.Max(entry => entry.Notification.Width);

            point.X -= width;

            for (var i = Entries.Count - 1; i >= 0; --i)
            {
                var entry = Entries[i];
                var deltaTime = entry.Time - time;

                var bgcolor = Theme.BackgroundColor;
                var bcolor = Theme.BorderColor;
                var txtcolor = Theme.TextColor;

                if (deltaTime < 0f)
                {
                    Entries.RemoveAt(i);
                    continue;
                }

                if (deltaTime < decayTime)
                {
                    var stage = deltaTime / decayTime;

                    void Decay(ref Color color) => color.A = (byte)(color.A * stage);

                    Decay(ref bgcolor);
                    Decay(ref bcolor);
                    Decay(ref txtcolor);
                }

                var bpoint = point;
                var bsizes = new List<Size2>();

                if (entry.Notification.Header != null)
                {
                    var hsize = entry.Notification.headerSize;
                    hsize.Width = width;

                    bsizes.Add(hsize);

                    Theme.DrawTextBox(point, hsize, bgcolor, txtcolor, entry.Notification.Header, true);
                    point.Y += entry.Notification.headerSize.Height;
                }

                var csize = entry.Notification.contentSize;
                csize.Width = width;

                bsizes.Add(csize);

                Theme.DrawTextBox(point, csize, bgcolor, txtcolor, entry.Notification.Content);

                if (borders)
                {
                    Theme.DrawBorders(bpoint, bcolor, bsizes.ToArray());
                }

                point.Y += csize.Height + Theme.MinItemHeight;
            }
        }

        internal static void UpdateAllSizes()
        {
            Entries.ForEach(entry => entry.Notification.UpdateSizes());
        }

        private class NotificationEntry
        {
            public readonly Notification Notification;

            public readonly float Time;

            public NotificationEntry(Notification n, float duration)
            {
                this.Notification = n;

                this.Time = duration + GetTime();
            }
        }
    }
}