using System;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class ProductionComponent : Component
    {
        // the recipe for this generator
        public string Recipe { get; set; }
    }
}
