using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace IsoECS.DataStructures
{
    [Serializable]
    public class DrawableSprite : IGameDrawable
    {
        public string SpriteSheet { get; set; }
        public string ID { get; set; }

        public string Layer { get; set; }
        public bool Visible { get; set; }
        public Color Color { get; set; }
        public float Alpha { get; set; }
        public float Rotation { get; set; }
        public SpriteEffects Effects { get; set; }

        // does this move when the camera moves?
        public bool Static { get; set; }

        public DrawableSprite()
        {
            Visible = true;
            Color = Color.White;
            Static = false;
            Alpha = 1.0f;
        }

        public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, SpriteFont font, float positionX, float positionY)
        {

            // update the destination on the drawable
            Rectangle source = Textures.Instance.GetSource(SpriteSheet, ID);
            Rectangle destination;

            Color targetColor = new Color()
            {
                R = Color.R,
                G = Color.G,
                B = Color.B,
                A = (byte)(Color.A * Alpha)
            };

            if (Static)
                destination = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
            else
                destination = new Rectangle((int)positionX, (int)positionY, source.Width, source.Height);

            // TODO: needs to take into account the origin
            if (graphics.Viewport.Bounds.Contains(destination) || graphics.Viewport.Bounds.Intersects(destination))
                spriteBatch.Draw(Textures.Instance.Get(SpriteSheet), destination, source, targetColor, Rotation, Textures.Instance.GetOrigin(SpriteSheet, ID), Effects, 0);
        }
    }
}
