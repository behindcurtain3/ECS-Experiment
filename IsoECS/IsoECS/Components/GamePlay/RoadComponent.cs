using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    public class RoadComponent : Component
    {
        public string RoadType { get; set; }
        public Point BuiltAt { get; set; }
    }
}
