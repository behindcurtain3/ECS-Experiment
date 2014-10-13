using System;
using IsoECS.DataStructures.Json.Converters;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace IsoECS.Components
{
    [Serializable]
    public class PositionComponent : Component
    {
        public float X { get; set; }
        public float Y { get; set; }

        [JsonConverter(typeof(PointConverter))]
        public Point Index { get; set; }

        public string GenerateAt { get; set; }

        public Vector2 Position 
        {
            get { return new Vector2(X, Y); }
        }

        public PositionComponent()
        {
            X = 0;
            Y = 0;
        }

        public PositionComponent(Vector2 position)
        {
            X = position.X;
            Y = position.Y;                
        }

        public PositionComponent(float x, float y)
        {
            X = x;
            Y = y;
        }

    }
}
