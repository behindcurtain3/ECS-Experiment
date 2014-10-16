using System;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class MoveToTargetComponent : Component
    {
        public int TargetID { get; set; }

        public Point Target { get; set; }
        public Path PathToTarget { get; set; }
        public float Speed { get; set; }
    }
}
