using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class CollisionMapComponent : Component
    {
        // holds the value of the collision at each map index (point)
        // -1 impassable
        public Dictionary<Point, int> Map { get; set; }

        public CollisionMapComponent()
        {
            Map = new Dictionary<Point, int>();
        }

    }
}
