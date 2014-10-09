using System;
using IsoECS.DataStructures.Json.Converters;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace IsoECS.DataStructures
{
    [Serializable]
    public class LocationValue
    {
        public int Value { get; set; }

        [JsonConverter(typeof(PointConverter))]
        public Point Offset { get; set; }
    }
}
