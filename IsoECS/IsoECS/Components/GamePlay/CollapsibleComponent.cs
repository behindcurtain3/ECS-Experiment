using TecsDotNet;

namespace IsoECS.Components.GamePlay
{
    public class CollapsibleComponent : Component
    {
        // 0 = collapsed, 100 = maxxed out
        // reduces over time
        public double Status { get; set; }
    }
}
