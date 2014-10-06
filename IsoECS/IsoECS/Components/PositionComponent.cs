using Microsoft.Xna.Framework;

namespace IsoECS.Components
{
    public class PositionComponent : Component
    {
        public float X { get; set; }
        public float Y { get; set; }

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
