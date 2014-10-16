using System;
using System.Collections.Generic;
using IsoECS.Behaviors;

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

        // TODO: switch to a different component + use a behavior manager/brain
        public Stack<Behavior> Behaviors { get; set; }

        public CitizenComponent()
        {
            FatherID = -1;
            MotherID = -1;
            HousingID = -1;

            Behaviors = new Stack<Behavior>();
        }
    }
}
