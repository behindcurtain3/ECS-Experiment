using Microsoft.Xna.Framework;

namespace IsoECS.DataStructures
{
    public class PathWaypoint
    {
        public PathWaypoint Parent { get; set; }
        public PathWaypoint Child { get; set; }
        public Point Location { get; set; }

        // used in calculating paths
        public int Length { get; set; }
        public int Cost { get; set; }

        public PathWaypoint(Point location, PathWaypoint parent = null)
        {
            Length = 0;
            Cost = 10;
            Location = location;
            SetParent(parent);
        }

        public void SetParent(PathWaypoint parent)
        {
            if (parent == null)
                Length = 0;
            else
            {
                parent.Child = this;
                Parent = parent;

                Length = Parent.Length + Cost;
            }
        }
    }
}
