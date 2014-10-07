using System.Collections.Generic;
using IsoECS.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.Systems
{
    public interface IRenderSystem
    {
        void Draw(List<Entity> entities, SpriteBatch spriteBatch, SpriteFont spriteFont);
    }
}
