using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.GamePlay;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class Inventory : Component
    {
        public Dictionary<string, int> Items { get; set; }

        public Inventory()
        {
            Items = new Dictionary<string, int>();
        }

        public bool Add(string name, int amount)
        {
            if (!Items.ContainsKey(name))
                Items.Add(name, 0);

            Items[name] += amount;

            // in future check for restrictions like a max amount of inventory space
            return true;
        }
    }
}
