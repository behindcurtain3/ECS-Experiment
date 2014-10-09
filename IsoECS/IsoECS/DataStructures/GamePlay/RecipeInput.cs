using System;

namespace IsoECS.DataStructures.GamePlay
{
    [Serializable]
    public class RecipeInput
    {
        public string Item { get; set; }
        public double Bonus { get; set; }
        public bool Required { get; set; }

        public RecipeInput()
        {
            Required = true;
        }
    }
}
