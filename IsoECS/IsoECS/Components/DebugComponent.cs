using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoECS.Components
{
    public class DebugComponent : Component
    {
        public int ID { get; set; }

        // what map location is the debug component over?
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}
