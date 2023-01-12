namespace Commodore.GameLogic.Core
{
    public class SystemMemoryAddresses
    {
        public const int CtrlPressState = 0x000000;                     // byte
        public const int ShiftPressState = 0x000001;                    // byte
        public const int AltPressState = 0x000002;                      // byte
        public const int SoftResetCompleteFlag = 0x000003;              // byte
        public const int BreakKeyScancode = 0x000006;                   // byte
        public const int RevertToTextModeScancode = 0x000007;           // byte
        public const int __reservedD = 0x000008;                        // word
        public const int __reservedE = 0x00000A;                        // byte
        public const int __reservedF = 0x00000B;                        // byte
        public const int CurrentForegroundColor = 0x00000C;             // dword
        public const int CurrentMarginColor = 0x000010;                 // dword
        public const int CurrentBackgroundColor = 0x000014;             // dword
        public const int UpdateOffsetParametersFlag = 0x000018;         // byte
        public const int CursorPositionX = 0x000019;                    // dword
        public const int CursorPositionY = 0x00001D;                    // dword
        public const int MarginArea = 0x000021;                         // dword
        public const int __reservedG = 0x000025;                        // dword

        // ----------------------------------------------------------------------
        public const int UserDataArea = 0x000600;
    }
}