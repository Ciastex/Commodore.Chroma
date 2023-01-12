namespace Commodore.GameLogic.Core
{
    public class SystemMemoryAddresses
    {
        public const int __reservedA = 0x000000;                        // dword
        public const int __reservedB = 0x000004;                        // byte
        public const int __reservedC = 0x000005;                        // byte
        public const int BreakKeyScancode = 0x000006;                   // byte
        public const int RevertToTextModeScancode = 0x000007;           // byte
        public const int __reservedD = 0x000008;                        // word
        public const int CurrentMarginSize = 0x00000A;                  // byte
        public const int CurrentPaddingSize = 0x00000B;                 // byte
        public const int CurrentForegroundColor = 0x00000C;             // dword
        public const int CurrentMarginColor = 0x000010;                 // dword
        public const int CurrentBackgroundColor = 0x000014;             // dword
        public const int UpdateOffsetParametersFlag = 0x000018;         // byte
        public const int CursorPositionX = 0x000019;                    // dword
        public const int CursorPositionY = 0x00001D;                    // dword
        public const int CtrlPressState = 0x000021;                     // byte
        public const int ShiftPressState = 0x000022;                    // byte
        public const int SoftResetCompleteFlag = 0x000023;              // byte
        public const int __reservedE = 0x000024;                        // dword
        public const int __reservedF = 0x000028;                        // dword
        public const int Voice1Frequency = 0x00002C;                    // word
        public const int Voice1Generator = 0x00002E;                    // byte
        public const int Voice2Frequency = 0x00002F;                    // word
        public const int Voice2Generator = 0x000031;                    // byte
        public const int Voice3Frequency = 0x000032;                    // word
        public const int Voice3Generator = 0x000034;                    // byte
        public const int Voice4Frequency = 0x000035;                    // word
        public const int Voice4Generator = 0x000037;                    // byte
        public const int Voice5Frequency = 0x000038;                    // word
        public const int Voice5Generator = 0x00003A;                    // byte
        public const int Voice6Frequency = 0x00003B;                    // word
        public const int Voice6Generator = 0x00003D;                    // byte
        public const int Voice7Frequency = 0x00003E;                    // word
        public const int Voice7Generator = 0x000040;                    // byte
        public const int Voice8Frequency = 0x000041;                    // word
        public const int Voice8Generator = 0x000043;                    // byte

        // ----------------------------------------------------------------------
        public const int UserDataArea = 0x000600;
    }
}