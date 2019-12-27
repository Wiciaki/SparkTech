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

        public readonly string Content, Header;

        public Notification(string content, string header = null)
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

        public static void Send(string content, string header = null)
        {
            Send(content, header, 5f);
        }

        public static void Send(string content, string header, float duration)
        {
            new Notification(content, header).Send(duration);
        }

        private Size2 headerSize, contentSize;

        private void UpdateSizes()
        {
            this.contentSize = Theme.MeasureText(this.Content);

            this.headerSize = new Size2(this.contentSize.Width, Theme.MinItemHeight);
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
            var t = GetTime();
            var width = Entries.Max(entry => entry.Notification.contentSize.Width);

            point.X -= width;

            for (var i = Entries.Count - 1; i >= 0; --i)
            {
                var entry = Entries[i];
                var delta = entry.Time - t;

                var bgColor = Theme.BackgroundColor;
                var bcolor = Theme.BorderColor;
                var textAlpha = byte.MaxValue;

                if (delta < 0)
                {
                    Entries.RemoveAt(i);
                    continue;
                }

                if (delta < decayTime)
                {
                    var stage = delta / decayTime;

                    bgColor = DecayColor(bgColor, stage);
                    bcolor = DecayColor(bcolor, stage);
                    textAlpha = (byte)(stage * byte.MaxValue);
                }

                var bpoint = point;
                var bsizes = new List<Size2>();

                if (entry.Notification.Header != null)
                {
                    var headerSize = entry.Notification.headerSize;
                    headerSize.Width = width;

                    bsizes.Add(headerSize);

                    Theme.DrawTextBox(point, headerSize, bgColor, entry.Notification.Header, true, textAlpha);

                    point.Y += entry.Notification.headerSize.Height;
                }

                var contentSize = entry.Notification.contentSize;
                contentSize.Width = width;

                bsizes.Add(contentSize);

                Theme.DrawTextBox(point, contentSize, bgColor, entry.Notification.Content, false, textAlpha);

                if (borders)
                {
                    Theme.DrawBorders(bpoint, bcolor, bsizes.ToArray());
                }

                point.Y += contentSize.Height + Theme.MinItemHeight;
            }
        }

        private static Color DecayColor(Color color, float decayStage)
        {
            color.A = (byte)(color.A * decayStage);

            return color;
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

                this.Time = duration + decayTime + GetTime();
            }
        }
    }
}