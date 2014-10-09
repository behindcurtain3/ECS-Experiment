using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoECS.GamePlay
{
    [Serializable]
    public class Item : IUnique
    {
        public string UniqueID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public double Value { get; set; }
        public string SpriteSheet { get; set; }
        public string SourceID { get; set; }
    }
}
