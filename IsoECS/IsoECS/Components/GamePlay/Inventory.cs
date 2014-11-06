using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.GamePlay;
using TecsDotNet;

namespace IsoECS.Components.GamePlay
{
    public class InventoryEventArgs : EventArgs
    {
        public InventoryData Data { get; private set; }
        public int AmountChanged { get; private set; }

        public InventoryEventArgs(InventoryData data, int amount)
            : base()
        {
            Data = data;
            AmountChanged = amount;
        }
    }

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
        public delegate void InventoryEventHandler(Inventory sender, InventoryEventArgs e);

        public event InventoryEventHandler AmountChanged;

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

            if (AmountChanged != null)
                AmountChanged.Invoke(this, new InventoryEventArgs(Items[name], amount));

            // in future check for restrictions like a max amount of inventory space
            return true;
        }

        public int Get(string name)
        {
            if (!Items.ContainsKey(name))
                return 0;

            return Items[name].Amount;
        }

        public int Available(string name)
        {
            if (!Items.ContainsKey(name))
                return 0;

            if (!Items[name].Output)
                return 0;

            return Items[name].Amount;
        }

        public void Set(string name, int amount)
        {
            if (!Items.ContainsKey(name))
                Items.Add(name, new InventoryData() { Item = name });

            Items[name].Amount = amount;

            if (AmountChanged != null)
                AmountChanged.Invoke(this, new InventoryEventArgs(Items[name], amount));
        }
    }
}
