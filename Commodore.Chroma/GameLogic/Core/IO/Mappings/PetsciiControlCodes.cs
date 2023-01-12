using Chroma.Input;
using System.Collections.Generic;

namespace Commodore.GameLogic.Core.IO.Mappings
{
    public static class PetsciiControlCodes
    {
        public static Dictionary<KeyCode, char> PetsciiSymbolMappings = new Dictionary<KeyCode, char>
        {
            { KeyCode.Q, '\ue05e' }, { KeyCode.W, '\ue05f' }, { KeyCode.E, '\ue060' }, { KeyCode.R, '\ue061' }, { KeyCode.T, '\ue062' }, { KeyCode.Y, '\ue063' }, { KeyCode.U, '\ue064' }, { KeyCode.I, '\ue065' }, { KeyCode.O, '\ue066' }, { KeyCode.P, '\ue067' }, { KeyCode.LeftBracket, '\ue068' }, { KeyCode.RightBracket, '\ue069' },
            { KeyCode.A, '\ue06a' }, { KeyCode.S, '\ue06b' }, { KeyCode.D, '\ue06c' }, { KeyCode.F, '\ue06d' }, { KeyCode.G, '\ue06e' }, { KeyCode.H, '\ue06f' }, { KeyCode.J, '\ue070' }, { KeyCode.K, '\ue071' }, { KeyCode.L, '\ue072' }, { KeyCode.Semicolon, '\ue073' }, {KeyCode.Quote, '\ue074' }, { KeyCode.Plus, '\ue075' },
            { KeyCode.Z, '\ue076' }, { KeyCode.X, '\ue077' }, { KeyCode.C, '\ue078' }, { KeyCode.V, '\ue079' }, { KeyCode.B, '\ue07a' }, { KeyCode.N, '\ue07b' }, { KeyCode.M, '\ue07c' }, { KeyCode.Comma, '\ue07d' }, { KeyCode.Period, '\ue07e' }, { KeyCode.Slash, '\ue07f' },
        };

        public static Dictionary<KeyCode, char> PetsciiSymbolMappingShifted = new Dictionary<KeyCode, char>
        {
            { KeyCode.Q, '\ue2a0' }, { KeyCode.W, '\ue0a1' }, { KeyCode.E, '\ue0a2' }, { KeyCode.R, '\ue0a3' }, { KeyCode.T, '\ue0a4' }, { KeyCode.Y, '\ue0a5' }, { KeyCode.U, '\ue0a6' }, { KeyCode.I, '\ue0a7' }, { KeyCode.O, '\ue0a8' }, { KeyCode.P, '\ue0a9' }, { KeyCode.LeftBracket, '\ue0aa' }, { KeyCode.RightBracket, '\ue0ab' },
            { KeyCode.A, '\ue0ac' }, { KeyCode.S, '\ue0ad' }, { KeyCode.D, '\ue0ae' }, { KeyCode.F, '\ue0af' }, { KeyCode.G, '\ue0b0' }, { KeyCode.H, '\ue0b1' }, { KeyCode.J, '\ue0b2' }, { KeyCode.K, '\ue0b3' }, { KeyCode.L, '\ue0b4' }, { KeyCode.Semicolon, '\ue0b5' }, { KeyCode.Quote, '\ue0b6' }, { KeyCode.Plus, '\ue0b7' },
            { KeyCode.Z, '\ue0b8' }, { KeyCode.X, '\ue0b9' }, { KeyCode.C, '\ue0ba' }, { KeyCode.V, '\ue0bb' }, { KeyCode.B, '\ue0bc' }, { KeyCode.N, '\ue0bd' }, { KeyCode.M, '\ue0be' }, { KeyCode.Comma, '\ue0bf' }, { KeyCode.Period, '\ue05c' }, { KeyCode.Slash, '\ue07f' },
        };
    }
}
