using System.Collections.Generic;
using IsoECS.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.Systems
{
    public interface IRenderSystem
    {
        void Init();
        void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont);
        void Shutdown();
    }
}
