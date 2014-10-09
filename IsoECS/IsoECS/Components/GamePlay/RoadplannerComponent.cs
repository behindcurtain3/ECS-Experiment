using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class RoadPlannerComponent : Component
    {
        public Dictionary<Point, string> Built { get; private set; }

        public RoadPlannerComponent()
        {
            Built = new Dictionary<Point, string>();
        }
    }
}
