using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework;

namespace IsoECS.Util
{
    public class Pathwalker
    {
        private static List<Point> _roadsConnected = new List<Point>();

        public static List<Point> GetRoadsWithinDistance(RoadPlannerComponent roadPlanner, Point start, int distance)
        {
            _roadsConnected.Clear();
            GetRoads(roadPlanner, start, distance);

            return _roadsConnected;
        }

        private static void GetRoads(RoadPlannerComponent roadPlanner, Point current, int distance)
        {
            if (distance <= 0)
                return;

            // if the current point doesn't have a road return empty list
            if (!roadPlanner.IsRoadAt(current))
                return;

            if (_roadsConnected.Contains(current))
                return;

            _roadsConnected.Add(current);

            // check each ortho location
            GetRoads(roadPlanner, new Point(current.X - 1, current.Y), distance - 1);
            GetRoads(roadPlanner, new Point(current.X, current.Y - 1), distance - 1);
            GetRoads(roadPlanner, new Point(current.X + 1, current.Y), distance - 1);
            GetRoads(roadPlanner, new Point(current.X, current.Y + 1), distance - 1);
        }
    }
}
