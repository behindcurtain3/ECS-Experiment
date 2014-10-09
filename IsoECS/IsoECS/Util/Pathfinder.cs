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

            if (!collisionMap.Collision.ContainsKey(end))
                collisionMap.Collision.Add(end, 64);
            if (!collisionMap.Collision.ContainsKey(start))
                collisionMap.Collision.Add(start, 64);

            if (start == end || collisionMap.Collision[end] == -1 || collisionMap.Collision[start] == -1)
                return new Path();

            _openList.Add(end, new PathWaypoint(end, collisionMap.Collision[end]));

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

                        if (!Isometric.ValidIndex(map, selectedNode.Location.X + x, selectedNode.Location.Y + y))
                            continue;

                        Point p = new Point(selectedNode.Location.X + x, selectedNode.Location.Y + y);
                        if (!collisionMap.Collision.ContainsKey(p))
                            collisionMap.Collision.Add(p, 64);

                        PathWaypoint waypoint = new PathWaypoint(
                                                            p,
                                                            collisionMap.Collision[p],
                                                            selectedNode
                                                            );

                        if (collisionMap.Collision[waypoint.Location] != -1 && !_closedList.ContainsKey(waypoint.Location))
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
