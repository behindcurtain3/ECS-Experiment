using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.GamePlay;

namespace IsoECS.Components.GamePlay
{
    public class Inventory : Component
    {
        public Dictionary<string, int> Items { get; set; }

        public Inventory()
        {
            Items = new Dictionary<string, int>();
        }
    }
}
