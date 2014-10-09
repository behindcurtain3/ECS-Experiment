using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using IsoECS.DataStructures;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class FoundationComponent : Component
    {
        public List<LocationValue> Plan { get; set; }
        public string PlanType { get; set; }

        public FoundationComponent()
        {
            Plan = new List<LocationValue>();
            PlanType = "Normal";
        }
    }
}
