using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class CollisionMapComponent : Component
    {
        // holds the value of the collision at each map index (point)
        // -1 impassable
        public Dictionary<Point, int> Collision { get; set; }

        public CollisionMapComponent()
        {
            Collision = new Dictionary<Point, int>();
        }

    }
}
