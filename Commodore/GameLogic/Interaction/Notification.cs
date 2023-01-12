using System;
using System.Drawing;
using System.Numerics;
using Chroma.Graphics;
using Commodore.Framework;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Core.IO;
using Color = System.Drawing.Color;

namespace Commodore.GameLogic.Interaction
{
    public class Notification
    {
        private enum Animation
        {
            SlidingIn,
            Waiting,
            SlidingOut
        }

        private readonly Vector2 _targetBasePosition;
        private readonly float _waitTime = 4500f;

        private Animation _currentAnimation;
        private float _currentHorizontalPosition;
        private float _currentVerticalPosition;
        private float _currentWaitTimer;

        public bool IsActive { get; set; }
        public string Text { get; set; }
        public Size TextSize { get; }
        public Color BorderColor { get; set; }
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }

        public Notification(string text, Color borderColor, Color backgroundColor, Color textColor)
        {
            Text = text.ToUpper();
            TextSize = Kernel.Instance.Vga.Font.Measure(text);

            BorderColor = borderColor;
            BackgroundColor = backgroundColor;
            TextColor = textColor;

            _targetBasePosition = new Vector2(
                G.Window.Size.Width - TextSize.Width - (4 * 16) - 16,
                G.Window.Size.Height - TextSize.Height - (4 * 16) - 16
            );

            _currentHorizontalPosition = G.Window.Size.Width;
            _currentVerticalPosition = _targetBasePosition.Y;
            
            _currentAnimation = Animation.SlidingIn;
        }

        public void Draw(RenderContext context)
        {
            if (!IsActive) return;

            context.ShapeBlendingEnabled = false;
            context.Rectangle(
                ShapeMode.Fill,
                new Vector2(
                    _currentHorizontalPosition,
                    _currentVerticalPosition
                ),
                new Size(4 * 16, 4 * 16) + TextSize,
                BorderColor
            );
            
            context.Rectangle(
                ShapeMode.Fill,
                new Vector2(
                    _currentHorizontalPosition + 16,
                    _currentVerticalPosition + 16
                ),
                new Size(2 * 16, 2 * 16) + TextSize,
                BackgroundColor
            );
            context.ShapeBlendingEnabled = true;
            
            context.DrawString(
                Kernel.Instance.Vga.Font,
                Text,
                new Vector2(
                    _currentHorizontalPosition + 32,
                    _currentVerticalPosition + 32
                ),
                TextColor
            );
        }

        public void Update(float delta)
        {
            if (!IsActive) return;

            if (_currentAnimation == Animation.SlidingIn)
            {
                if (_currentHorizontalPosition > _targetBasePosition.X)
                {
                    _currentHorizontalPosition -= 16;
                }
                else
                {
                    _currentAnimation = Animation.Waiting;
                }
            }
            else if (_currentAnimation == Animation.Waiting)
            {
                if (_currentWaitTimer < _waitTime)
                {
                    _currentWaitTimer += 1000f * delta;
                }
                else
                {
                    _currentAnimation = Animation.SlidingOut;
                }
            }
            else if (_currentAnimation == Animation.SlidingOut)
            {
                if (_currentHorizontalPosition < G.Window.Size.Width)
                {
                    _currentHorizontalPosition += 16;
                }
                else
                {
                    IsActive = false;
                }
            }
        }
    }
}