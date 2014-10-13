using System;
using System.Collections.Generic;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class HousingComponent : Component
    {
        // The entity ID's for the occupants of this domicile
        public List<int> Tennants { get; set; }

        // what kind of/level of housing is this?
        public string Category { get; set; }

        // the max number of occupants for this domicile
        public int MaxOccupants { get; set; }

        // rent per month each tennant is charged
        public int Rent { get; set; }

        public HousingComponent()
        {
            Tennants = new List<int>();
        }
    }
}
