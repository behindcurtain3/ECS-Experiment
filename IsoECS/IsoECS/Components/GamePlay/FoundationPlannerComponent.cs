using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class FoundationPlannerComponent : Component
    {
        public Dictionary<Point, bool> SpaceTaken { get; set; }

        public FoundationPlannerComponent()
        {
            SpaceTaken = new Dictionary<Point, bool>();
        }
    }
}
