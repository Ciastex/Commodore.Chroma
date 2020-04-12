using Chroma.Graphics;
using System.Collections.Generic;

namespace Commodore.GameLogic.Core.IO.Mappings
{
    public static class ColorMappings
    {
        private static Dictionary<char, Color> _unicodeToColorMap = new Dictionary<char, Color>
        {
            { '\uff00', new Color(0, 0, 0) },
            { '\uff01', new Color(0, 0, 170) },
            { '\uff02', new Color(0, 170, 0) },
            { '\uff03', new Color(0, 170, 170) },
            { '\uff04', new Color(170, 0, 0) },
            { '\uff05', new Color(170, 0, 170) },
            { '\uff06', new Color(170, 170, 0) },
            { '\uff07', new Color(170, 170, 170) },

            { '\uff08', new Color(0, 0, 85) },
            { '\uff09', new Color(0, 0, 255) },
            { '\uff0a', new Color(0, 170, 85) },
            { '\uff0b', new Color(0, 170, 255) },
            { '\uff0c', new Color(170, 0, 85) },
            { '\uff0d', new Color(170, 0, 255) },
            { '\uff0e', new Color(170, 170, 85) },
            { '\uff0f', new Color(170, 170, 255) },

            { '\uff10', new Color(0, 85, 0) },
            { '\uff11', new Color(0, 85, 170) },
            { '\uff12', new Color(0, 255, 0) },
            { '\uff13', new Color(0, 255, 170) },
            { '\uff14', new Color(170, 85, 0) },
            { '\uff15', new Color(170, 85, 170) },
            { '\uff16', new Color(170, 255, 0) },
            { '\uff17', new Color(170, 255, 170) },

            { '\uff18', new Color(0, 85, 85) },
            { '\uff19', new Color(0, 85, 255) },
            { '\uff1a', new Color(0, 255, 85) },
            { '\uff1b', new Color(0, 255, 255) },
            { '\uff1c', new Color(170, 85, 85) },
            { '\uff1d', new Color(170, 85, 255) },
            { '\uff1e', new Color(170, 255, 85) },
            { '\uff1f', new Color(170, 255, 255) },

            { '\uff20', new Color(85, 0, 0) },
            { '\uff21', new Color(85, 0, 170) },
            { '\uff22', new Color(85, 170, 0) },
            { '\uff23', new Color(85, 170, 170) },
            { '\uff24', new Color(255, 0, 0) },
            { '\uff25', new Color(255, 0, 170) },
            { '\uff26', new Color(255, 170, 0) },
            { '\uff27', new Color(255, 170, 170) },

            { '\uff28', new Color(85, 0, 85) },
            { '\uff29', new Color(85, 0, 255) },
            { '\uff2a', new Color(85, 170, 85) },
            { '\uff2b', new Color(85, 170, 255) },
            { '\uff2c', new Color(255, 0, 85) },
            { '\uff2d', new Color(255, 0, 255) },
            { '\uff2e', new Color(255, 170, 85) },
            { '\uff2f', new Color(255, 170, 255) },

            { '\uff30', new Color(85, 85, 0) },
            { '\uff31', new Color(85, 85, 170) },
            { '\uff32', new Color(85, 255, 0) },
            { '\uff33', new Color(85, 255, 170) },
            { '\uff34', new Color(255, 85, 0) },
            { '\uff35', new Color(255, 85, 170) },
            { '\uff36', new Color(255, 255, 0) },
            { '\uff37', new Color(255, 255, 170) },
                  
            { '\uff38', new Color(85, 85, 85) },
            { '\uff39', new Color(85, 85, 255) },
            { '\uff3a', new Color(85, 255, 85) },
            { '\uff3b', new Color(85, 255, 255) },
            { '\uff3c', new Color(255, 85, 85) },
            { '\uff3d', new Color(255, 85, 255) },
            { '\uff3e', new Color(255, 255, 85) },
            { '\uff3f', new Color(255, 255, 255) },
        };

        public static bool IsForegroundMapping(char c) => _unicodeToColorMap.ContainsKey(c);
        public static bool IsBackgroundMapping(char c) => _unicodeToColorMap.ContainsKey((char)(c + 0x100));

        public static Color GetForeground(char c)
        {
            if (_unicodeToColorMap.ContainsKey(c))
                return _unicodeToColorMap[c];

            return Color.Black;
        }

        public static Color GetBackground(char c)
        {
            if (_unicodeToColorMap.ContainsKey((char)(c + 0x100)))
                return _unicodeToColorMap[(char)(c + 0x100)];

            return Color.Black;
        }
    }
}
