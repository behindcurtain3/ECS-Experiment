using System;
using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using Newtonsoft.Json.Linq;

namespace IsoECS.GamePlay
{
    [Serializable]
    public class Scenario
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string Textures { get; set; }
        public string Drawables { get; set; }
        public string Library { get; set; }
        public string Recipes { get; set; }

        public IEnumerable<JObject> DefaultEntities { get; set; }
    }
}
