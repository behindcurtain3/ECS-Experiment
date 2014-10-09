using IsoECS.DataStructures.Json.Converters;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace IsoECS.DataStructures.Json.Deserializable
{
    public class DeserializablePoint
    {
        [JsonConverter(typeof(PointConverter))]
        public Point Value { get; set; }
    }
}
