using System;
using System.Collections.Generic;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class HousingComponent : Component
    {
        // The entity ID's for the occupants of this domicile
        public List<int> Tennants { get; set; }

        public int MaxOccupants { get; set; }

        public HousingComponent()
        {
            Tennants = new List<int>();
        }
    }
}
