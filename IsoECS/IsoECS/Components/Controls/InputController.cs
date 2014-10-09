using System;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Components
{
    [Serializable]
    public class InputController : Component
    {
        public KeyboardState PrevKeyboard { get; set; }
        public KeyboardState CurrentKeyboard { get; set; }
        public MouseState PrevMouse { get; set; }
        public MouseState CurrentMouse { get; set; }
    }
}
