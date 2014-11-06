using System;
using Newtonsoft.Json.Linq;
using TecsDotNet.Json;

namespace IsoECS.DataStructures.Json.Converters
{
    public class DrawableConverter : JsonCreationConverter<GameDrawable>
    {
        protected override GameDrawable Create(Type objectType, JObject jObject)
        {
            if (FieldExists("SpriteSheet", jObject))
            {
                return new DrawableSprite();
            }
            else
            {
                return new DrawableText();
            }
        }

        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }
}
