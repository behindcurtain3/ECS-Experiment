using System;
using Microsoft.Xna.Framework;

namespace IsoECS.Components
{
    public class DrawableTextComponent : Component
    {
        public String Text { get; set; }
        public Color Color { get; set; }
    }
}
