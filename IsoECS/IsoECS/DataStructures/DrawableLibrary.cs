using System.Collections.Generic;
using System.IO;
using IsoECS.Components;
using IsoECS.DataStructures.Json;
using IsoECS.DataStructures.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IsoECS.Util;

namespace IsoECS.DataStructures
{
    public sealed class DrawableLibrary
    {
        private static readonly DrawableLibrary _instance = new DrawableLibrary();

        public static DrawableLibrary Instance
        {
            get { return _instance; }
        }

        // stores ready made entities by their unique ID
        private Dictionary<string, IGameDrawable> _drawables;

        private DrawableLibrary()
        {
            _drawables = new Dictionary<string, IGameDrawable>();
        }

        public IGameDrawable Get(string uniqueID)
        {
            return Serialization.DeepCopy<IGameDrawable>(_drawables[uniqueID]);
        }

        public void Add(JObject o)
        {
            DrawableComponent component = JsonConvert.DeserializeObject<DrawableComponent>(o.ToString(), new DrawableConverter());

            foreach (KeyValuePair<string, List<IGameDrawable>> kvp in component.Drawables)
            {
                foreach (IGameDrawable drawable in kvp.Value)
                {
                    drawable.Layer = kvp.Key;

                    if (string.IsNullOrWhiteSpace(drawable.UniqueID))
                        continue;

                    _drawables.Add(drawable.UniqueID, drawable);
                }
            }
        }

        public void LoadFromJson(string path, bool config = false)
        {
            string json = File.ReadAllText(path);

            if (config)
            {
                JsonListConfig jsonConfig = JsonConvert.DeserializeObject<JsonListConfig>(json);
                foreach (string str in jsonConfig.List)
                {
                    LoadFromJson(str);
                }
            }
            else
            {
                JObject fileObj = JObject.Parse(json);
                foreach (JObject o in fileObj["DrawableComponents"].ToObject<IEnumerable<JObject>>())
                {
                    Add(o);
                }
            }
        }
    }
}
