using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class CollisionMapComponent : Component
    {
        public Dictionary<Point, int> Collision { get; set; }

        public CollisionMapComponent()
        {
            Collision = new Dictionary<Point, int>();
        }

    }
}
