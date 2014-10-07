using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class RoadplannerComponent : Component
    {
        public Dictionary<Point, string> Built { get; private set; }

        public RoadplannerComponent()
        {
            Built = new Dictionary<Point, string>();
        }
    }
}
