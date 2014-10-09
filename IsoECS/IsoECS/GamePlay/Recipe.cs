using System;
using System.Collections.Generic;
using IsoECS.DataStructures.GamePlay;

namespace IsoECS.GamePlay
{
    [Serializable]
    public class Recipe : IUnique
    {
        public string UniqueID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<RecipeInput> Inputs { get; set; }
        public List<RecipeOutput> Outputs { get; set; }

        public Recipe()
        {
            Inputs = new List<RecipeInput>();
            Outputs = new List<RecipeOutput>();
        }
    }
}
