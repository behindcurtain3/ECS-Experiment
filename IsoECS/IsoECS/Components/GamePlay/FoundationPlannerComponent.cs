using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class FoundationPlannerComponent : Component
    {
        public Dictionary<Point, int> SpaceTaken { get; set; }

        public FoundationPlannerComponent()
        {
            SpaceTaken = new Dictionary<Point, int>();
        }
    }
}
