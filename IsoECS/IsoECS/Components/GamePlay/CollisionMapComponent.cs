using System.Collections.Generic;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class CollisionMapComponent : Component
    {
        // holds the value of the collision at each map index (point)
        // -1 impassable
        public Dictionary<Point, PathTypes> Map { get; set; }

        public CollisionMapComponent()
        {
            Map = new Dictionary<Point, PathTypes>();
        }

    }
}
