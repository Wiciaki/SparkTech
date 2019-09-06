namespace SparkTech.SDK
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Key
    {
        Backspace = 8,

        Tab = 9,

        Clear = 12, // 0x0000000C

        Enter = 13, // 0x0000000D

        Shift = 16, // 0x00000010

        Control = 17, // 0x00000011

        Alt = 18, // 0x00000012

        Pause = 19, // 0x00000013

        CapsLock = 20, // 0x00000014

        Escape = 27, // 0x0000001B

        Space = 32, // 0x00000020

        PageUp = 33, // 0x00000021

        PageDown = 34, // 0x00000022

        End = 35, // 0x00000023

        Home = 36, // 0x00000024

        Left = 37, // 0x00000025

        Up = 38, // 0x00000026

        Right = 39, // 0x00000027

        Down = 40, // 0x00000028

        Select = 41, // 0x00000029

        Print = 42, // 0x0000002A

        Execute = 43, // 0x0000002B

        PrintScreen = 44, // 0x0000002C

        Insert = 45, // 0x0000002D

        Delete = 46, // 0x0000002E

        Help = 47, // 0x0000002F

        Zero = 48, // 0x00000030

        One = 49, // 0x00000031

        Two = 50, // 0x00000032

        Three = 51, // 0x00000033

        Four = 52, // 0x00000034

        Five = 53, // 0x00000035

        Six = 54, // 0x00000036

        Seven = 55, // 0x00000037

        Eight = 56, // 0x00000038

        Nine = 57, // 0x00000039

        A = 65, // 0x00000041

        B = 66, // 0x00000042

        C = 67, // 0x00000043

        D = 68, // 0x00000044

        E = 69, // 0x00000045

        F = 70, // 0x00000046

        G = 71, // 0x00000047

        H = 72, // 0x00000048

        I = 73, // 0x00000049

        J = 74, // 0x0000004A

        K = 75, // 0x0000004B

        L = 76, // 0x0000004C

        M = 77, // 0x0000004D

        N = 78, // 0x0000004E

        O = 79, // 0x0000004F

        P = 80, // 0x00000050

        Q = 81, // 0x00000051

        R = 82, // 0x00000052

        S = 83, // 0x00000053

        T = 84, // 0x00000054

        U = 85, // 0x00000055

        V = 86, // 0x00000056

        W = 87, // 0x00000057

        X = 88, // 0x00000058

        Y = 89, // 0x00000059

        Z = 90, // 0x0000005A

        LeftWindowsKey = 91, // 0x0000005B

        RightWindowsKey = 92, // 0x0000005C

        ApplicationsKey = 93, // 0x0000005D

        Sleep = 95, // 0x0000005F

        NumPad0 = 96, // 0x00000060

        NumPad1 = 97, // 0x00000061

        NumPad2 = 98, // 0x00000062

        NumPad3 = 99, // 0x00000063

        NumPad4 = 100, // 0x00000064

        NumPad5 = 101, // 0x00000065

        NumPad6 = 102, // 0x00000066

        NumPad7 = 103, // 0x00000067

        NumPad8 = 104, // 0x00000068

        NumPad9 = 105, // 0x00000069

        Multiply = 106, // 0x0000006A

        Add = 107, // 0x0000006B

        Seperator = 108, // 0x0000006C

        Subtract = 109, // 0x0000006D

        Decimal = 110, // 0x0000006E

        Divide = 111, // 0x0000006F

        F1 = 112, // 0x00000070

        F2 = 113, // 0x00000071

        F3 = 114, // 0x00000072

        F4 = 115, // 0x00000073

        F5 = 116, // 0x00000074

        F6 = 117, // 0x00000075

        F7 = 118, // 0x00000076

        F8 = 119, // 0x00000077

        F9 = 120, // 0x00000078

        F10 = 121, // 0x00000079

        F11 = 122, // 0x0000007A

        F12 = 123, // 0x0000007B

        F13 = 124, // 0x0000007C

        F14 = 125, // 0x0000007D

        F15 = 126, // 0x0000007E

        F16 = 127, // 0x0000007F

        F17 = 128, // 0x00000080

        F18 = 129, // 0x00000081

        F19 = 130, // 0x00000082

        F20 = 131, // 0x00000083

        F21 = 132, // 0x00000084

        F22 = 133, // 0x00000085

        F23 = 134, // 0x00000086

        F24 = 135, // 0x00000087

        Numlock = 144, // 0x00000090

        ScrollLock = 145, // 0x00000091

        LeftShift = 160, // 0x000000A0

        RightShift = 161, // 0x000000A1

        LeftControl = 162, // 0x000000A2

        RightContol = 163, // 0x000000A3

        LeftMenu = 164, // 0x000000A4

        RightMenu = 165, // 0x000000A5

        BrowserBack = 166, // 0x000000A6

        BrowserForward = 167, // 0x000000A7

        BrowserRefresh = 168, // 0x000000A8

        BrowserStop = 169, // 0x000000A9

        BrowserSearch = 170, // 0x000000AA

        BrowserFavorites = 171, // 0x000000AB

        BrowserHome = 172, // 0x000000AC

        VolumeMute = 173, // 0x000000AD

        VolumeDown = 174, // 0x000000AE

        VolumeUp = 175, // 0x000000AF

        NextTrack = 176, // 0x000000B0

        PreviousTrack = 177, // 0x000000B1

        StopMedia = 178, // 0x000000B2

        PlayPause = 179, // 0x000000B3

        LaunchMail = 180, // 0x000000B4

        SelectMedia = 181, // 0x000000B5

        LaunchApp1 = 182, // 0x000000B6

        LaunchApp2 = 183, // 0x000000B7

        OEMPlus = 184, // 0x000000B8

        OEM1 = 186, // 0x000000BA

        OEMComma = 188, // 0x000000BC

        OEMMinus = 189, // 0x000000BD

        OEMPeriod = 190, // 0x000000BE

        OEM2 = 191, // 0x000000BF

        OEM3 = 192, // 0x000000C0

        OEM4 = 219, // 0x000000DB

        OEM5 = 220, // 0x000000DC

        OEM6 = 221, // 0x000000DD

        OEM7 = 222, // 0x000000DE

        OEM8 = 223, // 0x000000DF

        OEM102 = 226, // 0x000000E2

        Process = 229, // 0x000000E5

        Packet = 231, // 0x000000E7

        Attn = 246, // 0x000000F6

        CrSel = 247, // 0x000000F7

        ExSel = 248, // 0x000000F8

        EraseEOF = 249, // 0x000000F9

        Play = 250, // 0x000000FA

        Zoom = 251, // 0x000000FB

        PA1 = 253, // 0x000000FD

        OEMClear = 254, // 0x000000FE
    }
}