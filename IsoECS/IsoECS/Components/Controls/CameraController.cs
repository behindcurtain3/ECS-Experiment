using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Components
{
    [Serializable]
    public class CameraController : Component
    {
        public List<Keys> Up { get; set; }
        public List<Keys> Down { get; set; }
        public List<Keys> Left { get; set; }
        public List<Keys> Right { get; set; }

        public CameraController()
        {
            Up = new List<Keys>();
            Down = new List<Keys>();
            Left = new List<Keys>();
            Right = new List<Keys>();
        }
    }
}
