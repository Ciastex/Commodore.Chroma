using Chroma.Graphics;

namespace Commodore.Framework.Extensions
{
    public static class RenderTargetExtensions
    {
        public static void Clear(this RenderTarget renderTarget)
        {
            for (var y = 0; y < renderTarget.Height; y++)
            {
                for (var x = 0; x < renderTarget.Width; x++)
                {
                    renderTarget[x, y] = Color.Transparent;
                }
            }
            
            renderTarget.Flush();
        }
    }
}