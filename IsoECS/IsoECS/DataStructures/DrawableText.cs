using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.DataStructures
{
    [Serializable]
    public class DrawableText : GameDrawable
    {
        public string Text { get; set; }
        
        public DrawableText() 
            : base()
        {
            Color = Color.White;
            Visible = true;
            Static = true;
            Alpha = 1.0f;
        }

        public override void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, SpriteFont font, float cameraX, float cameraY)
        {
            Color targetColor = new Color()
            {
                R = Color.R,
                G = Color.G,
                B = Color.B,
                A = (byte)(Color.A * Alpha)
            };

            spriteBatch.DrawString(font, Text, new Vector2(cameraX, cameraY), targetColor);
        }
    }
}
