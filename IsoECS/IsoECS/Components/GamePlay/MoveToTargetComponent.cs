using System;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class MoveToTargetComponent : Component
    {
        public Point Target { get; set; }
        public Path PathToTarget { get; set; }
    }
}
