using System;
using TecsDotNet;

namespace IsoECS.GamePlay
{
    [Serializable]
    public class Item : Prototype
    {
        // display name
        public string Name { get; set; }

        // description to show in tooltip
        public string Description { get; set; }

        // used to calculate the cities networth and trade value of the good
        public double Value { get; set; }

        // can this item be stored in a stockpile?
        public bool StockpileEnabled { get; set; }

        // can this item be stored in a granary?
        public bool GranaryEnabled { get; set; }

        public string SpriteSheet { get; set; }
        public string SourceID { get; set; }
    }
}
