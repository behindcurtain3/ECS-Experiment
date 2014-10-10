using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class SpawnerComponent : Component
    {
        public int LimitPerMonth { get; set; }
        public List<string> Spawns { get; set; }

        public SpawnerComponent()
        {
            LimitPerMonth = 10;
            Spawns = new List<string>();
        }
    }
}
