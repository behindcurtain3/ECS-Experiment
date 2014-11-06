using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TecsDotNet;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class FoundationPlannerComponent : Component
    {
        public Dictionary<Point, uint> SpaceTaken { get; set; }

        public FoundationPlannerComponent()
        {
            SpaceTaken = new Dictionary<Point, uint>();
        }
    }
}
