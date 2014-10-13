using System;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class CitizenComponent : Component
    {
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public int FatherID { get; set; }
        public int MotherID { get; set; }

        public int Age { get; set; }
        public int Money { get; set; }

        // ID of the entity this citizen lives at
        public int HousingID { get; set; }

        public CitizenComponent()
        {
            FatherID = -1;
            MotherID = -1;
            HousingID = -1;
        }
    }
}
