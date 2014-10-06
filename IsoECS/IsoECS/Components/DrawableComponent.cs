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

        // the color
        public Color Color { get; set; }

        // sorting
        public int Layer { get; set; }

        // is this currently visible?
        public bool Visible { get; set; }
    }
}
