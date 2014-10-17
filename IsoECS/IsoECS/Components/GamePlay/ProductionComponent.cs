using System;
using System.Collections.Generic;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class ProductionComponent : Component
    {
        // the recipe for this generator
        public string Recipe { get; set; }

        public List<int> Employees { get; set; }
        public int MaxEmployees { get; set; }

        public ProductionComponent()
        {
            Employees = new List<int>();
        }
    }
}
