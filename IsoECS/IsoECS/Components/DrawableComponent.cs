using System;
using System.Collections.Generic;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.Components
{
    [Serializable]
    public class DrawableComponent : Component
    {
        // sorting
        public int Layer { get; set; }

        public List<DrawableSprite> Sprites { get; set; }

        public DrawableComponent()
        {
            Sprites = new List<DrawableSprite>();
            Layer = 1;
        }
    }
}
