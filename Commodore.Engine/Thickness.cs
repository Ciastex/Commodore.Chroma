using System;

namespace Commodore.Engine
{
    [Serializable]
    public class Thickness
    {
        public float Left = 0;
        public float Top = 0;
        public float Right = 0;
        public float Bottom = 0;

        public Thickness(float uniform)
        {
            Left = Top = Right = Bottom = uniform;
        }

        public Thickness(float horizontal, float vertical)
        {
            Left = Right = horizontal;
            Top = Bottom = vertical;
        }

        public Thickness(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
