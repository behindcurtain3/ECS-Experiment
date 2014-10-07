using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Components
{
    public class InputController : Component
    {
        public KeyboardState PrevKeyboard { get; set; }
        public KeyboardState CurrentKeyboard { get; set; }
        public MouseState PrevMouse { get; set; }
        public MouseState CurrentMouse { get; set; }
    }
}
