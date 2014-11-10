using System;
using System.Collections.Generic;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;

namespace IsoECS.GamePlay
{
    [Serializable]
    public class ServiceData
    {
        // measures the availability of water
        public int WaterAvailability { get; set; }

        // measures the desireability of the area
        public int Desireability { get; set; }

        public string RoadType { get; set; }
        public uint FoundationOwner { get; set; }
        public PathTypes CollisionType { get; set; }

        public ServiceData()
        {
            WaterAvailability = 0;
            Desireability = 0;
        }
    }

    [Serializable]
    public class Services : Dictionary<Point, ServiceData>
    {
        public Services()
        {
        }
    }
}
