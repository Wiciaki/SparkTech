namespace SparkTech.SDK.League
{
    using SharpDX;

    public struct ProjectionInfo
    {
        public bool IsOnSegment { get; }
        
        public Vector2 LinePoint { get; }

        public Vector2 SegmentPoint { get; }

        public ProjectionInfo(bool isOnSegment, Vector2 segmentPoint, Vector2 linePoint)
        {
            this.IsOnSegment = isOnSegment;
            this.SegmentPoint = segmentPoint;
            this.LinePoint = linePoint;
        }
    }
}