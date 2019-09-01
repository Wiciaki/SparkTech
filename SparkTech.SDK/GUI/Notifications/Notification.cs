namespace SparkTech.SDK.GUI.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    using SparkTech.SDK.Game;
    using SparkTech.SDK.Rendering;

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
            Render.OnEndScene += OnEndScene;
        }

        private static void OnEndScene()
        {
            var point = new Point(1870, 100);

            var t = GameInterface.Time();

            var width = Entries.Max(entry => entry.Notification.contentSize.Width);

            point.X -= width;

            foreach (var entry in Entries)
            {
                var color = entry.GetColor(t);

                if (entry.Notification.Header != null)
                {
                    var headerSize = entry.Notification.headerSize;
                    headerSize.Width = width;

                    Theme.DrawTextBox(point, headerSize, entry.Notification.Header, true, color);
                    Theme.DrawBorders(point, headerSize);

                    point.Y += entry.Notification.headerSize.Height;
                }

                var contentSize = entry.Notification.contentSize;
                contentSize.Width = width;

                Theme.DrawTextBox(point, contentSize, entry.Notification.Content, false, color);
                Theme.DrawBorders(point, contentSize);

                point.Y += contentSize.Height + Theme.MinItemHeight;
            }
        }

        internal static void UpdateAllSizes()
        {
            Entries.ForEach(n => n.Notification.UpdateSizes());
        }

        private class NotificationEntry
        {
            private const float DecayTime = 2f;

            public readonly Notification Notification;

            private float time;

            public Color GetColor(float gameTime)
            {
                if (this.time + DecayTime > gameTime)
                {
                    
                }

                return Theme.BackgroundColor;
            }

            public NotificationEntry(Notification n, float duration)
            {
                this.Notification = n;

                this.time = duration + GameInterface.Time();
            }
        }

        /*
        internal static bool DrawClock;

        internal static Size ExtraClockSize;

        private static Size clockTextSize;

        private static readonly List<Notification> ActiveNotifications = new List<Notification>();

        static Notification()
        {
            Render.OnEndScene += OnEndScene;
            Theme.Proxy.ThemeSelected += UpdateItemSizes;
        }

        private Notification Copy(float time)
        {
            return new Notification { Content = this.Content, Description = this.Description, Time = this.Time + time };
        }

        private static string ClockText => DateTime.Now.ToShortTimeString();

        private static void UpdateItemSizes()
        {
            clockTextSize = Theme.MeasureSize(ClockText);

            var l = new List<Notification>();

            foreach (var notification in ActiveNotifications)
            {
                notification.UpdateSizes();

                l.Add(notification);
            }

            ActiveNotifications.Clear();
            ActiveNotifications.AddRange(l);
        }

        private Size descriptionSize, contentSize;

        public string Description;

        public string Content;

        public float Time;

        public bool Send()
        {
            if (string.IsNullOrWhiteSpace(this.Content))
            {
                throw new ArgumentException("Notification content was null or white space prior to sending!");
            }

            this.UpdateSizes();

            var left = Render.Resolution().Height - clockTextSize.Height - 40;

            void Add(Notification n) => left -= n.contentSize.Height + n.descriptionSize.Height + Theme.ItemDistance.Height + 20;

            ActiveNotifications.ForEach(Add);
            Add(this);

            if (left < 0)
            {
                return false;
            }

            ActiveNotifications.Add(this.Copy(this.Time + Game.ClockTime));

            return true;
        }

        public static bool Send(float time, string content, string description = null)
        {
            return new Notification { Time = time, Content = content, Description = description }.Send();
        }

        private void UpdateSizes()
        {
            this.contentSize = Theme.MeasureSize(this.Content);

            this.descriptionSize = Theme.MeasureSize(this.Description);
        }

        private static void OnEndScene()
        {
            var reservedWidth = ActiveNotifications.Select(n => Math.Max(n.contentSize.Width, n.descriptionSize.Width)).Max();
            var offset = Render.Resolution().Width - 40;
            var point = new Point(offset - reservedWidth, 40);

            if (DrawClock && Menu.IsOpen)
            {
                var clockPoint = new Point(offset - clockTextSize.Width, 40) + ExtraClockSize;

                Theme.Draw(new DrawData(clockPoint, clockTextSize) { Text = ClockText });

                point.Y += clockTextSize.Height + ExtraClockSize.Height + 10;
            }

            var clockTime = Game.ClockTime;
            var updatedTime = clockTime + 3f;

            for (var i = 0; i < ActiveNotifications.Count; i++)
            {
                var n = ActiveNotifications[i];

                var time = n.Time - clockTime;
                var fontColor = Theme.FontColor;
                var bgColor = Theme.BackgroundColor;

                const float FadeoutTime = 1.75f;

                if (time < -FadeoutTime)
                {
                    ActiveNotifications.RemoveAt(i--);
                    continue;
                }

                if (time < 0f)
                {
                    var alphaMod = (time + FadeoutTime) / FadeoutTime;

                    void SetTransparency(ref Color c) => c = Color.FromArgb((byte)(c.A * alphaMod), c);

                    SetTransparency(ref fontColor);
                    SetTransparency(ref bgColor);
                }

                point.Y += 20;

                if (n.Description != null)
                {
                    n.descriptionSize.Width = reservedWidth;

                    InteractCheck(n.descriptionSize);
                    Theme.Draw(new DrawData(point, n.descriptionSize) { Text = n.Description, ForceTextCentered = true, Bold = true, FontColor = fontColor, BackgroundColor = bgColor });

                    point.Y += n.descriptionSize.Height;
                }

                n.contentSize.Width = reservedWidth;
                InteractCheck(n.contentSize);

                Theme.Draw(new DrawData(point, n.contentSize) { Text = n.Content, FontColor = fontColor, BackgroundColor = bgColor });

                point.Y += n.contentSize.Height + Theme.ItemDistance.Height;

                void InteractCheck(Size size)
                {
                    if (updatedTime > n.Time && MenuComponent.Mouse.IsInside(point, size))
                    {
                        ActiveNotifications[i] = n.Copy(updatedTime);
                    }
                }
            }
        }*/
    }
}