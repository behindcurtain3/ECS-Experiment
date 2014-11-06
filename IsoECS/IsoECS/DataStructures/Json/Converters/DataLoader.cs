using Newtonsoft.Json.Linq;
using TecsDotNet;
using TecsDotNet.Json;
using Newtonsoft.Json;

namespace IsoECS.DataStructures.Json.Converters
{
    public class DataLoader<T> : JsonPrototypeLoader
    {
        public override Prototype LoadPrototype(JObject source)
        {
            T data = JsonConvert.DeserializeObject<T>(source.ToString());

            return (Prototype)(object)data;
        }
    }
}
