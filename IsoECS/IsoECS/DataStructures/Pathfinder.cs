using System;
using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;

namespace IsoECS.DataStructures
{
    public enum PathTypes
    {
        BLOCKED = -1,
        ROAD = 8,
        UNDEFINED = 64
    }

    public class Pathfinder
    {
        private Dictionary<Point, PathWaypoint> _openList = new Dictionary<Point, PathWaypoint>();
        private Dictionary<Point, PathWaypoint> _closedList = new Dictionary<Point, PathWaypoint>();

        public Path Generate(CollisionMapComponent collisionMap, IsometricMapComponent map, Point start, Point end)
        {
            // reset the lists
            _openList.Clear();
            _closedList.Clear();

            if (!collisionMap.Map.ContainsKey(end))
                collisionMap.Map.Add(end, PathTypes.UNDEFINED);
            if (!collisionMap.Map.ContainsKey(start))
                collisionMap.Map.Add(start, PathTypes.UNDEFINED);

            if (start == end || collisionMap.Map[end] == PathTypes.BLOCKED || collisionMap.Map[start] == PathTypes.BLOCKED)
                return new Path();

            _openList.Add(end, new PathWaypoint(end, (int)collisionMap.Map[end]));

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

                        if (!map.IsValidIndex(selectedNode.Location.X + x, selectedNode.Location.Y + y))
                            continue;

                        Point p = new Point(selectedNode.Location.X + x, selectedNode.Location.Y + y);
                        if (!collisionMap.Map.ContainsKey(p))
                            collisionMap.Map.Add(p, PathTypes.UNDEFINED);

                        PathWaypoint waypoint = new PathWaypoint(
                                                            p,
                                                            (int)collisionMap.Map[p],
                                                            selectedNode
                                                            );

                        if (collisionMap.Map[waypoint.Location] != PathTypes.BLOCKED && !_closedList.ContainsKey(waypoint.Location))
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

        public Path Generate(CollisionMapComponent collisionMap, IsometricMapComponent map, Point start, List<Point> end)
        {
            // reset the lists
            _openList.Clear();
            _closedList.Clear();

            foreach (Point p in end)
            {
                if (!collisionMap.Map.ContainsKey(p))
                    collisionMap.Map.Add(p, PathTypes.UNDEFINED);
            }
            if (!collisionMap.Map.ContainsKey(start))
                collisionMap.Map.Add(start, PathTypes.UNDEFINED);

            if (collisionMap.Map[start] == PathTypes.BLOCKED)
                return new Path();

            _openList.Add(start, new PathWaypoint(start));

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

                foreach (Point p in end)
                {
                    if (selectedNode.Location == p)
                        return new Path(selectedNode, start, p, true);
                }

                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if (x != 0 && y != 0)
                            continue;

                        // no diags or self
                        if (x == y || x == -y || -x == y)
                            continue;

                        if (!map.IsValidIndex(selectedNode.Location.X + x, selectedNode.Location.Y + y))
                            continue;

                        Point p = new Point(selectedNode.Location.X + x, selectedNode.Location.Y + y);
                        if (!collisionMap.Map.ContainsKey(p))
                            collisionMap.Map.Add(p, PathTypes.UNDEFINED);

                        PathWaypoint waypoint = new PathWaypoint(
                                                            p,
                                                            (int)collisionMap.Map[p],
                                                            selectedNode
                                                            );

                        if (collisionMap.Map[waypoint.Location] != PathTypes.BLOCKED && !_closedList.ContainsKey(waypoint.Location))
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
