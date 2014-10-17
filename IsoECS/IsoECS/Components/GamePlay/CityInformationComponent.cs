using System;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class CityInformationComponent : Component
    {
        public String Name { get; set; }

        public int Money { get; set; }
        public int Population { get; set; }
    }
}
