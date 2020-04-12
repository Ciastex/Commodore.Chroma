using System;

namespace Commodore.Engine
{
    public class Time
    {
        public static uint Stamp => (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
}
