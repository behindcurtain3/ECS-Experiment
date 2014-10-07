using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.Components
{
    public class DrawableComponent : Component
    {
        // the texture to draw
        public Texture2D Texture { get; set; }

        // which part of the source texture to draw
        public Rectangle Source { get; set; }

        // destination rectangle
        public Rectangle Destination { get; set; }

        // the color
        public Color Color { get; set; }

        // sorting
        public int Layer { get; set; }

        // is this currently visible?
        public bool Visible { get; set; }

        // any sprite effects
        public SpriteEffects Effects { get; set; }

        // origin
        public Vector2 Origin { get; set; }

        //rotation
        public float Rotation { get; set; }

        // does this move when the camera moves?
        public bool Static { get; set; }

        public DrawableComponent()
        {
            Visible = true;
            Color = Color.White;
            Layer = 1;
            Static = false;
        }
    }
}
