using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.DataStructures
{
    public interface IGameDrawable
    {
        bool Visible { get; set; }

        void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, float cameraX, float cameraY);
    }
}
