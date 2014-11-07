using System;
using System.Collections.Generic;
using System.Linq;
using IsoECS.DataStructures;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using TecsDotNet;
using IsoECS.GamePlay;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class RoadPlannerComponent : Component
    {
        public Dictionary<Point, string> Built { get; private set; }

        public RoadPlannerComponent()
        {
            Built = new Dictionary<Point, string>();
        }

        public bool IsRoadAt(Point p)
        {
            return Built.ContainsKey(p);
        }

        public void AddOrUpdate(GameWorld world, Entity e, bool recursion = false)
        {
            RoadComponent road = e.Get<RoadComponent>();
            string value = "dirt-road-" + GetRoadValue(world.Map, road.BuiltAt);

            // set the value of built into the roadPlanner
            if (!Built.ContainsKey(road.BuiltAt))
                Built.Add(road.BuiltAt, value);
            else
                Built[road.BuiltAt] = value;

            if (road.Updateable)
            {
                DrawableComponent drawable = e.Get<DrawableComponent>();
                foreach (GameDrawable sprite in drawable.Drawables["Foundation"].ToList())
                {
                    if (sprite.PrototypeID.Contains("road"))
                    {
                        // remove old road drawables
                        drawable.Drawables["Foundation"].Remove(sprite);
                    }
                }
                road.RoadType = Built[road.BuiltAt];
                drawable.Add("Foundation", (GameDrawable)world.Prototypes[road.RoadType]);
            }

            if (recursion)
            {
                UpdateNeighbors(world, e);
            }
        }

        public void RemoveRoad(IsometricMapComponent map, Point location)
        {
            if (Built.ContainsKey(location))
                Built.Remove(location);

            //UpdateNeighbors(map, location);
        }

        private void UpdateNeighbors(GameWorld world, Entity e)
        {
            RoadComponent road = e.Get<RoadComponent>();
            Point location = new Point(road.BuiltAt.X - 1, road.BuiltAt.Y);

            // update the neighbors to account for this one
            if (world.Map.IsValidIndex(location) && IsRoadAt(location))
                AddOrUpdate(world, GetRoadAtOffset(world, e, -1, 0));

            location = new Point(road.BuiltAt.X, road.BuiltAt.Y - 1);

            if (world.Map.IsValidIndex(location) && IsRoadAt(location))
                AddOrUpdate(world, GetRoadAtOffset(world, e, 0, -1));

            location = new Point(road.BuiltAt.X + 1, road.BuiltAt.Y);

            if (world.Map.IsValidIndex(location) && IsRoadAt(location))
                AddOrUpdate(world, GetRoadAtOffset(world, e, 1, 0));

            location = new Point(road.BuiltAt.X, road.BuiltAt.Y + 1);

            if (world.Map.IsValidIndex(location) && IsRoadAt(location))
                AddOrUpdate(world, GetRoadAtOffset(world, e, 0, 1));
        }

        private string GetRoadValue(IsometricMapComponent map, Point location)
        {
            // set the value of the location to index of the type of road at this location
            bool NW = false;
            bool NE = false;
            bool SE = false;
            bool SW = false;

            if (map.IsValidIndex(location.X - 1, location.Y))
                NW = Built.ContainsKey(new Point(location.X - 1, location.Y));

            if (map.IsValidIndex(location.X, location.Y - 1))
                NE = Built.ContainsKey(new Point(location.X, location.Y - 1));

            if (map.IsValidIndex(location.X + 1, location.Y))
                SE = Built.ContainsKey(new Point(location.X + 1, location.Y));

            if (map.IsValidIndex(location.X, location.Y + 1))
                SW = Built.ContainsKey(new Point(location.X, location.Y + 1));

            // 1 combo of 4
            if (NW && NE && SE && SW)
                return "fourway";

            // four different combos of 3
            else if (NW && NE && SE)
                return "ne-threeway";
            else if (NW && NE && SW)
                return "nw-threeway";
            else if (NW && SE && SW)
                return "sw-threeway";
            else if (NE && SE && SW)
                return "se-threeway";

            // 6 combos of 2
            else if (SW && SE)
                return "bottom-corner";
            else if (NW && SW)
                return "left-corner";
            else if (NW && NE)
                return "top-corner";
            else if (NE && SE)
                return "right-corner";
            else if (SW && NE)
                return "diagonal-up";
            else if (NW && SE)
                return "diagonal-down";

            // 4 combos of 1
            else if (SW)
                return "sw-endpoint";
            else if (NW)
                return "nw-endpoint";
            else if (NE)
                return "ne-endpoint";
            else if (SE)
                return "se-endpoint";

            // 1 cobmo of 0
            else
                return "single";
        }

        private Entity GetRoadAtOffset(GameWorld world, Entity e, int x, int y)
        {
            RoadComponent road = e.Get<RoadComponent>();
            Point p = new Point(road.BuiltAt.X + x, road.BuiltAt.Y + y);

            uint id = world.Foundations.SpaceTaken[p];

            return world.Entities.Get(id);
        }
    }
}
