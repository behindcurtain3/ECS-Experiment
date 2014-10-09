using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.DataStructures
{
    public interface IGameDrawable
    {
        bool Visible { get; set; }
        int Layer { get; set; }
        bool Static { get; set; }

        void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, SpriteFont font, float cameraX, float cameraY);
    }
}
