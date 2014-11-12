using System;
using TecsDotNet;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class CollapsibleComponent : Component
    {
        public static double MAX = 100.0;

        // 0 = collapsed, 100 = maxxed out
        // reduces over time
        public double Value { get; set; }
    }
}
