using IsoECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.RenderSystems
{
    public class RenderSystem : GameSystem
    {
        public GraphicsDevice Graphics { get; set; }
        public Color ClearColor { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
        }
    }
}
