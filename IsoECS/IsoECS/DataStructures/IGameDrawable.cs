using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IsoECS.DataStructures
{
    public interface IGameDrawable
    {
        bool Visible { get; set; }
        int Layer { get; set; }
        bool Static { get; set; }
        Color Color { get; set; }
        float Alpha { get; set; }

        void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, SpriteFont font, float cameraX, float cameraY);
    }
}
