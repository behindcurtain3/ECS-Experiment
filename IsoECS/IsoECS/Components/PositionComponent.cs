using Microsoft.Xna.Framework;

namespace IsoECS.Components
{
    public class PositionComponent : Component
    {
        public Vector2 Position { get; set; }

        public PositionComponent()
        {
            Position = Vector2.Zero;
        }

        public PositionComponent(Vector2 position)
        {
            Position = position;
        }

    }
}
