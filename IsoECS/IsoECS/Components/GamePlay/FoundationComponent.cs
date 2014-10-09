using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class FoundationComponent : Component
    {
        public List<Point> FloorPlan { get; set; }

        public FoundationComponent()
        {
            FloorPlan = new List<Point>();
        }
    }
}
