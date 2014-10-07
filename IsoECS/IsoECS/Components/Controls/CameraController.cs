using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Components
{
    public class CameraController : Component
    {
        public List<Keys> Up { get; private set; }
        public List<Keys> Down { get; private set; }
        public List<Keys> Left { get; private set; }
        public List<Keys> Right { get; private set; }

        public CameraController()
        {
            Up = new List<Keys>();
            Down = new List<Keys>();
            Left = new List<Keys>();
            Right = new List<Keys>();
        }
    }
}
