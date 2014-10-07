using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class FloorplannerComponent : Component
    {
        public Dictionary<Point, bool> SpaceTaken { get; set; }

        public FloorplannerComponent()
        {
            SpaceTaken = new Dictionary<Point, bool>();
        }
    }
}
