using System;
using Microsoft.Xna.Framework;
using TecsDotNet;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class RoadComponent : Component
    {
        public string RoadType { get; set; }
        public Point BuiltAt { get; set; }
        public bool Updateable { get; set; }

        public RoadComponent()
        {
            Updateable = true;
        }
    }
}
