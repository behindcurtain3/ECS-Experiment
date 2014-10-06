using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using IsoECS.DataStructures.Json.Converters;

namespace IsoECS.DataStructures.Json
{
    public class JsonTexture
    {
        public string ID { get; set; }

        [JsonConverter(typeof(RectangleConverter))]
        public Rectangle Source { get; set; }
    }
}
