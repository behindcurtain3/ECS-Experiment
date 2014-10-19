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
        public Gender EmployeeGender { get; set; }

        public int CurrentStage { get; set; }
        public double WorkDone { get; set; }
        public long LastTick { get; set; }

        public ProductionComponent()
        {
            Employees = new List<int>();
            EmployeeGender = Gender.BOTH;
        }
    }
}
