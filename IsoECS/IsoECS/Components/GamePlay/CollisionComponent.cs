using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.DataStructures;

namespace IsoECS.Components.GamePlay
{
    public class CollisionComponent : Component
    {
        public List<LocationValue> Data { get; set; }

        public CollisionComponent()
        {
            Data = new List<LocationValue>();
        }
    }
}
