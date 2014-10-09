using System;

namespace IsoECS.DataStructures.GamePlay
{
    [Serializable]
    public class RecipeOutput
    {
        // the name of the item that is output
        public string Item { get; set; }

        // the amount per month per worker that is created in a standard situation
        public double Rate { get; set; }

        // if true this recipe constantly outputs at a fraction of the Rate
        // if false it outputs in a lump sum
        public bool ConstantProduction { get; set; }
    }
}
