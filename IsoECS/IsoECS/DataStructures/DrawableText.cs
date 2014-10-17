﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IsoECS.DataStructures
{
    [Serializable]
    public class DrawableText : IGameDrawable
    {
        public string Layer { get; set; }
        public bool Visible { get; set; }
        public bool Static { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public float Alpha { get; set; }

        public DrawableText()
        {
            Color = Color.White;
            Visible = true;
            Static = true;
            Alpha = 1.0f;
        }

        public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, SpriteFont font, float cameraX, float cameraY)
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
