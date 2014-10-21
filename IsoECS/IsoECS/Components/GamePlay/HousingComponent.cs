using System;
using System.Collections.Generic;
using IsoECS.DataStructures.GamePlay;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class HousingComponent : Component
    {
        // The entity ID's for the occupants of this domicile
        public List<int> Tennants { get; set; }

        // Any entities who have reserved a spot in this house
        public List<int> ProspectiveTennants { get; set; }

        // what kind of/level of housing is this?
        public string Category { get; set; }

        // the max number of occupants for this domicile
        public int MaxOccupants { get; set; }

        // number of current occupants
        public int NumOccupants { get { return Tennants.Count; } }

        public int NumOccupantsAndProspectives { get { return Tennants.Count + ProspectiveTennants.Count; } }

        // rent per month each tennant is charged
        public int Rent { get; set; }

        // unique id of the entity this house upgrades to
        public string UpgradesTo { get; set; }

        public UpgradeRequirements UpgradeRequirements { get; set; }

        public HousingComponent()
        {
            Tennants = new List<int>();
            ProspectiveTennants = new List<int>();
        }
    }
}
