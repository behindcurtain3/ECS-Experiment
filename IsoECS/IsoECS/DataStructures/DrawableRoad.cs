using System;

namespace IsoECS.DataStructures
{
    [Serializable]
    public class DrawableRoad : DrawableSprite
    {
        public bool Updateable { get; set; }

        public DrawableRoad()
        {
            Updateable = true;
        }
    }
}
