using System.Numerics;
using Chroma.Graphics;

namespace Commodore.GameLogic.Display
{
    public class Cursor
    {
        private bool _visible;
        private int _timer;

        public int X { get; private set; }
        public int Y { get; private set; }

        public int Granularity = 16;
        public int BlinkInterval = 500;
        public Color ColorMask = Color.White;
        public bool ForceVisible;
        public bool ForceHidden;

        public Cursor()
        {
            X = Y = 0;
        }

        public void SetPixelBasedPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetGranularPosition(int column, int row)
            => SetPixelBasedPosition(column * Granularity, row * Granularity);

        public void Draw(RenderContext context)
        {
            if (_visible)
            {
                context.SetShapeBlendingPreset(BlendingPreset.Add);
                context.Rectangle(ShapeMode.Fill, new Vector2(X, Y), Granularity, Granularity, ColorMask);
            }
        }

        public void Reset()
        {
            _timer = 0;
            _visible = true;
        }

        public void Update(float deltaTime)
        {
            if (ForceHidden)
            {
                _visible = false;
                return;
            }

            if (ForceVisible)
            {
                _visible = true;
                return;
            }

            if (_timer >= BlinkInterval)
            {
                _visible = !_visible;
                _timer = 0;
                
                return;
            }

            _timer += (int)(2000 * deltaTime);
        }
    }
}