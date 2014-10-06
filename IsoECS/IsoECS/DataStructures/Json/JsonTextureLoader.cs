using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoECS.DataStructures.Json
{
    public class JsonTextureLoader
    {
        public string Texture { get; set; }
        public string Name { get; set; }
        public JsonTexture[] Sources { get; set; }
    }
}
