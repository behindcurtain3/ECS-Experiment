using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.GamePlay;

namespace IsoECS.Components.GamePlay
{
    public class Generator : Component
    {
        // the recipe for this generator
        public Recipe Recipe { get; set; }

        // the rate it is produced (in milliseconds)
        public int Rate { get; set; }

        // rate counter
        public int RateCountdown { get; set; }

    }
}
