using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IsoECS.DataStructures
{
    public class Path
    {
        public List<Point> Waypoints { get; private set; }
        public Point Start { get; set; }
        public Point End { get; set; }

        public Path()
        {
            Waypoints = new List<Point>();
        }

        public Path(PathWaypoint waypoint, Point start, Point end)
        {
            Waypoints = new List<Point>();

            Start = start;
            End = end;

            PathWaypoint p = waypoint;
            while (p != null)
            {
                Waypoints.Add(p.Location);
                p = p.Parent;
            }
        }
    }
}
