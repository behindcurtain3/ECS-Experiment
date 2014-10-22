using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.GamePlay;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class InventoryData
    {
        public string Item { get; set; }
        public int Amount { get; set; }
        public bool Output { get; set; }
        public bool Input { get; set; }

        public InventoryData()
        {
            Amount = 0;
            Output = false;
            Input = false;
        }
    }

    [Serializable]
    public class Inventory : Component
    {
        public Dictionary<string, InventoryData> Items { get; set; }

        public Inventory()
        {
            Items = new Dictionary<string, InventoryData>();
        }

        public bool Add(string name, int amount)
        {
            if (!Items.ContainsKey(name))
                Items.Add(name, new InventoryData() { Item = name });

            Items[name].Amount += amount;

            // in future check for restrictions like a max amount of inventory space
            return true;
        }

        public int Get(string name)
        {
            if (!Items.ContainsKey(name))
                return 0;

            return Items[name].Amount;
        }
    }
}
