using System;
using IsoECS.GamePlay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TecsDotNet;

namespace IsoECS.DataStructures
{
    [Serializable]
    public class GameDrawable : Prototype
    {
        public string Layer { get; set; }
        public bool Visible { get; set; }        
        public bool Static { get; set; }
        public Color Color { get; set; }
        public float Alpha { get; set; }

        public virtual void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, SpriteFont font, float cameraX, float cameraY) { }
    }
}
