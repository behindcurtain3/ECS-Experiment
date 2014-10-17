﻿using System;
using System.Collections.Generic;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.Components
{
    [Serializable]
    public class DrawableComponent : Component
    {
        public List<IGameDrawable> Drawables { get; set; }

        public List<DrawableSprite> Sprites { get; set; }
        public List<DrawableText> Texts { get; set; }

        public DrawableComponent()
        {
            Drawables = new List<IGameDrawable>();
            Sprites = new List<DrawableSprite>();
            Texts = new List<DrawableText>();
        }
    }
}
