using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.DataStructures;
using TecsDotNet;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class CollisionComponent : Component
    {
        public List<LocationValue> Plan { get; set; }
        public string PlanType { get; set; }

        public CollisionComponent()
        {
            Plan = new List<LocationValue>();
            PlanType = "Normal"; // default value
        }
    }
}
