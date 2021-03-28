namespace SparkTech.SDK.Entities
{
    public enum SpellDataTargetType
    {
        Unknown = -1, // 0xFFFFFFFF
        Self = 0,
        Target = 1,
        Area = 2,
        Cone = 3,
        SelfAoe = 4,
        TargetOrLocation = 5,
        Location = 6,
        Direction = 7,
        DragDirection = 8,
        LineTargetToCaster = 9,
        AreaClamped = 10, // 0x0000000A
        LocationClamped = 11, // 0x0000000B
        TerrainLocation = 12, // 0x0000000C
        TerrainType = 13, // 0x0000000D
    }
}