namespace SparkTech.SDK
{
    using System;

    public class PlatformFixes
    {
        public int WatermarkOffset { get; set; }

        public Action PostLoadAction { get; set; }
    }
}