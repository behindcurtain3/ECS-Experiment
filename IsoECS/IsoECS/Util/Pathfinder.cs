using System;
using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;

namespace IsoECS.Util
{
    public class Pathfinder
    {
        private static Dictionary<Point, PathWaypoint> _openList = new Dictionary<Point, PathWaypoint>();
        private static Dictionary<Point, PathWaypoint> _closedList = new Dictionary<Point, PathWaypoint>();

        public static Path Generate(CollisionMapComponent collisionMap, IsometricMapComponent map, Point start, Point end)
        {
            // reset the lists
            _openList.Clear();
            _closedList.Clear();

            if (start == end)
                return new Path();

            _openList.Add(end, new PathWaypoint(end));

            while (_openList.Count > 0)
            {
                int min = Int32.MaxValue;
                PathWaypoint selectedNode = null;

                foreach (PathWaypoint node in _openList.Values)
                {
                    int G = node.Length;
                    int H = MathHelper.Distance(start, node.Location);
                    int length = G + H;
                    if (length < min)
                    {
                        min = length;
                        selectedNode = node;
                    }
                }

                _openList.Remove(selectedNode.Location);
                _closedList.Add(selectedNode.Location, selectedNode);

                if (selectedNode.Location == start)
                    return new Path(selectedNode, start, end);

                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if (x != 0 && y != 0)
                            continue;

                        // no diags or self
                        if (x == y || x == -y || -x == y)
                            continue;

                        if (selectedNode.Location.X + x < 0 || selectedNode.Location.Y + y < 0 || selectedNode.Location.X + x >= map.TxWidth || selectedNode.Location.Y + y >= map.TxHeight)
                            continue;

                        PathWaypoint waypoint = new PathWaypoint(
                                                            new Point(selectedNode.Location.X + x, selectedNode.Location.Y + y), 
                                                            selectedNode);

                        // TODO: check the collision map to see if the location is walkable
                        if (!_closedList.ContainsKey(waypoint.Location))
                        {
                            if (_openList.ContainsKey(waypoint.Location))
                            {
                                if (_openList[waypoint.Location].Length > waypoint.Length)
                                    _openList[waypoint.Location].SetParent(waypoint.Parent);
                            }
                            else
                                _openList.Add(waypoint.Location, waypoint);
                        }
                    }
                }
            }

            return new Path();
        }
    }
}
