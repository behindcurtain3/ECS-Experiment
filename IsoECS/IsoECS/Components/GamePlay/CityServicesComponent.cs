using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class ServiceData
    {
        // measures the availability of water
        public int WaterAvailability { get; set; }

        // measures the desireability of the area
        public int Desireability { get; set; }

        public ServiceData()
        {
            WaterAvailability = 0;
            Desireability = 0;
        }
    }

    [Serializable]
    public class CityServicesComponent : Component
    {
        public Dictionary<Point, ServiceData> Data { get; set; }

        public CityServicesComponent()
        {
            Data = new Dictionary<Point, ServiceData>();
        }
    }
}
