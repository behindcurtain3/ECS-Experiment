using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.DataStructures
{
    [Serializable]
    public class DrawableSprite
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
    }
}
