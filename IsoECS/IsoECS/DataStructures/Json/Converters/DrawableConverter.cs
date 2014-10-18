using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace IsoECS.DataStructures.Json.Converters
{
    public class DrawableConverter : JsonCreationConverter<IGameDrawable>
    {
        protected override IGameDrawable Create(Type objectType, JObject jObject)
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
