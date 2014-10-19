using System;

namespace IsoECS.DataStructures.GamePlay
{
    [Serializable]
    public class RecipeOutput
    {
        // the name of the item that is output
        public string Item { get; set; }

        // the amount of the item produced
        public int AmountProduced { get; set; }
    }
}
