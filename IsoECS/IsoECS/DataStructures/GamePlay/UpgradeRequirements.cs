using System;

namespace IsoECS.DataStructures.GamePlay
{
    [Serializable]
    public class UpgradeRequirements
    {
        public int MinimumOccupants { get; set; }

        public UpgradeRequirements()
        {
            MinimumOccupants = -1;
        }
    }
}
