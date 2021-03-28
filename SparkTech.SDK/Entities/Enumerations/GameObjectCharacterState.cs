namespace SparkTech.SDK.Entities
{
    using System;

    [Flags]
    public enum GameObjectCharacterState
    {
        CanAttack = 1,

        CanCast = 2,

        CanMove = 8,

        Immovable = 4,

        IsStealth = 16, // 0x00000010

        RevealSpecificUnit = 32, // 0x00000020

        Taunted = 64, // 0x00000040

        Feared = 128, // 0x00000080

        Fleeing = 256, // 0x00000100

        Surpressed = 512, // 0x00000200

        Asleep = 1024, // 0x00000400

        NearSight = 2048, // 0x00000800

        Ghosted = 4096, // 0x00001000

        GhostProof = 8192, // 0x00002000

        Charmed = 16384, // 0x00004000

        NoRender = 32768, // 0x00008000

        DodgePiercing = 131072, // 0x00020000

        DisableAmbientGold = 262144, // 0x00040000

        DisableAmbientXp = 524288, // 0x00080000

        ForceRenderParticles = 65536, // 0x00010000
    }
}