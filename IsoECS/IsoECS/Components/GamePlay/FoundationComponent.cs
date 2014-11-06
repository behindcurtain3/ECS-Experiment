using System;
using System.Collections.Generic;
using IsoECS.DataStructures;
using TecsDotNet;

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
