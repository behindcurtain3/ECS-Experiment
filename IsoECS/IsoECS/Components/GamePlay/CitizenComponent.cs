using System;
using System.Collections.Generic;
using IsoECS.Behaviors;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public enum Gender
    {
        BOTH,
        MALE,
        FEMALE
    }

    [Serializable]
    public class CitizenComponent : Component
    {
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public Gender Gender { get; set; }
        public int FatherID { get; set; }
        public int MotherID { get; set; }

        public int Age { get; set; }
        public int Money { get; set; }

        // ID of the entity this citizen lives at
        public int HousingID { get; set; }

        // ID of the entity this citizen works at
        public int JobID { get; set; }

        // ID of the entity this citizen is "inside" (IE: inside a building)
        public int InsideID { get; set; }

        // TODO: switch to a different component + use a behavior manager/brain
        public Stack<Behavior> Behaviors { get; set; }

        public bool IsHauler { get; set; }

        public CitizenComponent()
        {
            FatherID = -1;
            MotherID = -1;
            HousingID = -1;
            JobID = -1;
            InsideID = -1;
            Gender = GamePlay.Gender.BOTH;
            IsHauler = false;

            Behaviors = new Stack<Behavior>();
        }
    }
}
