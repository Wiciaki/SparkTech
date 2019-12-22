namespace Surgical.SDK.GUI.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    using Surgical.SDK.Rendering;

    public class Notification
    {
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
            Send("HELLO TEST");

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
                var stage = entry.GetDecayStage(t);

                if (entry.Notification.Header != null)
                {
                    var headerSize = entry.Notification.headerSize;
                    headerSize.Width = width;

                    Theme.DrawTextBox(point, headerSize, entry.Notification.Header, true, Theme.BackgroundColor, stage);
                    //Theme.DrawBorders(point, headerSize);

                    point.Y += entry.Notification.headerSize.Height;
                }

                var contentSize = entry.Notification.contentSize;
                contentSize.Width = width;

                Theme.DrawTextBox(point, contentSize, entry.Notification.Content, false, Theme.BackgroundColor, stage);
                //Theme.DrawBorders(point, contentSize);

                point.Y += contentSize.Height + Theme.MinItemHeight;

                if (Math.Abs(stage) < 0.01f)
                {
                    Entries.RemoveAt(i);
                }
            }
        }

        internal static void UpdateAllSizes()
        {
            Entries.ForEach(entry => entry.Notification.UpdateSizes());
        }

        private class NotificationEntry
        {
            private const float DecayTime = 4f;

            public readonly Notification Notification;

            private readonly float time;

            public float GetDecayStage(float gameTime)
            {
                var delta = this.time - gameTime;

                return delta < DecayTime ? Math.Max(0f, delta / DecayTime) : 1f;
            }

            public NotificationEntry(Notification n, float duration)
            {
                this.Notification = n;

                this.time = duration + DecayTime + GetTime();
            }
        }
    }
}