using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TecsDotNet.Json;
using TecsDotNet;
using Newtonsoft.Json.Linq;
using IsoECS.Components;
using Newtonsoft.Json;

namespace IsoECS.DataStructures.Json.Converters
{
    public class DrawablesLoader : JsonPrototypeLoader
    {
        public DrawablesLoader()
            : base()
        {
            Identifier = "Drawables";
        }

        public override Prototype LoadPrototype(JObject source)
        {
            GameDrawable drawable = JsonConvert.DeserializeObject<GameDrawable>(source.ToString(), new DrawableConverter());

            return drawable;
        }
    }
}
