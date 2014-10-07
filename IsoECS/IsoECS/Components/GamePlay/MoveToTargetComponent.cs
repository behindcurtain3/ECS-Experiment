using IsoECS.DataStructures;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class MoveToTargetComponent : Component
    {
        public Point Target { get; set; }
        public Path PathToTarget { get; set; }
    }
}
