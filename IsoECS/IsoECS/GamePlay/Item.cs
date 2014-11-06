using System;
using TecsDotNet;

namespace IsoECS.GamePlay
{
    [Serializable]
    public class Item : Prototype
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public double Value { get; set; }
        public string SpriteSheet { get; set; }
        public string SourceID { get; set; }
    }
}
