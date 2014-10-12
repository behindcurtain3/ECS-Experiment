
namespace IsoECS.Components.GamePlay
{
    public class CitizenComponent : Component
    {
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public int FatherID { get; set; }
        public int MotherID { get; set; }

        public int Age { get; set; }
        public double Money { get; set; }
    }
}
