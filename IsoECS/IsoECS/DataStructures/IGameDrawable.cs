using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using IsoECS.GamePlay;

namespace IsoECS.DataStructures
{
    public interface IGameDrawable : IUnique
    {
        string Layer { get; set; }
        bool Visible { get; set; }        
        bool Static { get; set; }
        Color Color { get; set; }
        float Alpha { get; set; }

        void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, SpriteFont font, float cameraX, float cameraY);
    }
}
