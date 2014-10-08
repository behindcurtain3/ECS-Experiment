using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class FloorComponent : Component
    {
        public Point[] FloorPlan { get; set; }
    }
}
