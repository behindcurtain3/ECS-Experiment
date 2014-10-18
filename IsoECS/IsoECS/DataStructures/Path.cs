using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using IsoECS.DataStructures.Json.Converters;

namespace IsoECS.DataStructures
{
    [Serializable]
    public class Path
    {
        public List<Point> Waypoints { get; private set; }

        [JsonConverter(typeof(PointConverter))]
        public Point Start { get; set; }

        [JsonConverter(typeof(PointConverter))]
        public Point End { get; set; }

        public int Length { get; set; }

        public Path()
        {
            Waypoints = new List<Point>();
        }

        public Path(PathWaypoint waypoint, Point start, Point end, bool reversed = false)
        {
            Waypoints = new List<Point>();

            Start = start;
            End = end;
            Length = 0;

            PathWaypoint p = waypoint;
            while (p != null)
            {
                Waypoints.Add(p.Location);
                p = p.Parent;
                if (p != null)
                {
                    if (p.Length > Length)
                        Length = p.Length;
                }
            }

            // reverse the list
            if (reversed)
                Waypoints.Reverse();
        }
    }
}
