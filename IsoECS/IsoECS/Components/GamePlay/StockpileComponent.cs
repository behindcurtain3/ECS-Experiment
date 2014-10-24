using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class StockPileData
    {
        public delegate void StockpileEventHandler(StockPileData sender);
        public event StockpileEventHandler OnAmountChanged;

        private int _amount;

        public const int DefaultMax = 500;
        public const int DefaultMin = 0;

        public string Item { get; set; }
        public bool IsAccepting { get; set; }
        public int Maximum { get; set; }
        public int Minimum { get; set; }
        public int Amount 
        {
            get { return _amount; }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    if(OnAmountChanged != null)
                        OnAmountChanged.Invoke(this);
                }
            }
        }

        public StockPileData()
        {
            Maximum = DefaultMax;
            Minimum = DefaultMin;
            IsAccepting = true;
        }
    }

    [Serializable]
    public class StockpileComponent : Component
    {
        public Dictionary<string, StockPileData> StockPile { get; set; }

        public StockpileComponent()
        {
            StockPile = new Dictionary<string, StockPileData>();
        }

        public StockPileData Get(string item)
        {
            ValidateItem(item);

            return StockPile[item];
        }

        public bool IsAccepting(string item)
        {
            return StockPile.ContainsKey(item) ? StockPile[item].IsAccepting : true;
        }

        public int Amount(string item)
        {
            return StockPile.ContainsKey(item) ? StockPile[item].Amount : 0;
        }

        public int Minimum(string item)
        {
            return StockPile.ContainsKey(item) ? StockPile[item].Minimum : StockPileData.DefaultMin;
        }

        public int Maximum(string item)
        {
            return StockPile.ContainsKey(item) ? StockPile[item].Maximum : StockPileData.DefaultMax;
        }

        public void ToggleAccepting(string item)
        {
            ValidateItem(item);
            StockPile[item].IsAccepting = !StockPile[item].IsAccepting;
        }

        public void SetMinimum(string item, int value)
        {
            ValidateItem(item);
            StockPile[item].Minimum = value;
        }

        public void SetMaximum(string item, int value)
        {
            ValidateItem(item);
            StockPile[item].Maximum = value;
        }

        public int AddToItem(string item, int amount)
        {
            if (!IsAccepting(item))
                return 0;

            int max = Maximum(item);
            int already = Amount(item);
            int added = 0;
            if (max > 0)
            {
                int spaceLeft = (max - already);
                added = Math.Min(amount, spaceLeft);
            }

            ValidateItem(item);

            StockPile[item].Amount += added;
            return added;
        }
        
        private void ValidateItem(string item)
        {
            if (!StockPile.ContainsKey(item))
                StockPile.Add(item, new StockPileData() { Item = item });
        }
    }
}
