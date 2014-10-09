using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.DataStructures
{
    [Serializable]
    public class DrawableSprite : IGameDrawable
    {
        public string SpriteSheet { get; set; }
        public string ID { get; set; }

        public bool Visible { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public SpriteEffects Effects { get; set; }

        // does this move when the camera moves?
        public bool Static { get; set; }

        public DrawableSprite()
        {
            Visible = true;
            Color = Color.White;
            Static = false;
        }

        public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, float positionX, float positionY)
        {

            // update the destination on the drawable
            Rectangle source = Textures.Instance.GetSource(SpriteSheet, ID);
            Rectangle destination;

            if (Static)
                destination = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
            else
                destination = new Rectangle((int)positionX, (int)positionY, source.Width, source.Height);

            if (graphics.Viewport.Bounds.Contains(destination) || graphics.Viewport.Bounds.Intersects(destination))
                spriteBatch.Draw(Textures.Instance.Get(SpriteSheet), destination, source, Color, Rotation, Textures.Instance.GetOrigin(SpriteSheet, ID), Effects, 0);
        }
    }
}
