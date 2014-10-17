using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework;
using IsoECS.Entities;
using IsoECS.Components;
using IsoECS.DataStructures;

namespace IsoECS.Util
{
    public class RoadsHelper
    {

        public static void UpdateRoadsGfx(List<Entity> entities, RoadPlannerComponent planner)
        {
            // make sure the road has the right gfx & data
            foreach (Entity entity in entities)
            {
                RoadComponent road = entity.Get<RoadComponent>();
                DrawableComponent drawable = entity.Get<DrawableComponent>();

                // TODO: handle the removal of entity somewhere else
                if (!planner.Built.ContainsKey(road.BuiltAt))
                {
                    // remove the entity
                    entities.Remove(entity);
                }
                else
                {
                    if (road.Updateable)
                    {
                        road.RoadType = planner.Built[road.BuiltAt];
                        foreach (DrawableSprite sprite in drawable.Drawables)
                        {
                            sprite.ID = road.RoadType;
                        }
                    }
                }
            }
        }

        public static void AddOrUpdateRoad(RoadPlannerComponent planner, IsometricMapComponent map, Point location, bool recursion = false)
        {
            string value = GetRoadValue(planner, map, location);

            // set the value of built into the roadPlanner
            if (!planner.Built.ContainsKey(location))
                planner.Built.Add(location, value);
            else
                planner.Built[location] = value;

            if (recursion)
            {
                UpdateNeighbors(planner, map, location);
            }
        }

        public static void RemoveRoad(RoadPlannerComponent planner, IsometricMapComponent map, Point location)
        {
            if (planner.Built.ContainsKey(location))
                planner.Built.Remove(location);

            UpdateNeighbors(planner, map, location);
        }

        private static void UpdateNeighbors(RoadPlannerComponent planner, IsometricMapComponent map, Point location)
        {
            // update the neighbors to account for this one
            if (Isometric.ValidIndex(map, location.X - 1, location.Y) && planner.Built.ContainsKey(new Point(location.X - 1, location.Y)))
                AddOrUpdateRoad(planner, map, new Point(location.X - 1, location.Y));

            if (Isometric.ValidIndex(map, location.X, location.Y - 1) && planner.Built.ContainsKey(new Point(location.X, location.Y - 1)))
                AddOrUpdateRoad(planner, map, new Point(location.X, location.Y - 1));

            if (Isometric.ValidIndex(map, location.X + 1, location.Y) && planner.Built.ContainsKey(new Point(location.X + 1, location.Y)))
                AddOrUpdateRoad(planner, map, new Point(location.X + 1, location.Y));

            if (Isometric.ValidIndex(map, location.X, location.Y + 1) && planner.Built.ContainsKey(new Point(location.X, location.Y + 1)))
                AddOrUpdateRoad(planner, map, new Point(location.X, location.Y + 1));
        }

        private static string GetRoadValue(RoadPlannerComponent planner, IsometricMapComponent map, Point location)
        {
            // set the value of the location to index of the type of road at this location
            bool NW = false;
            bool NE = false;
            bool SE = false;
            bool SW = false;

            if (Isometric.ValidIndex(map, location.X - 1, location.Y))
                NW = planner.Built.ContainsKey(new Point(location.X - 1, location.Y));

            if (Isometric.ValidIndex(map, location.X, location.Y - 1))
                NE = planner.Built.ContainsKey(new Point(location.X, location.Y - 1));

            if (Isometric.ValidIndex(map, location.X + 1, location.Y))
                SE = planner.Built.ContainsKey(new Point(location.X + 1, location.Y));

            if (Isometric.ValidIndex(map, location.X, location.Y + 1))
                SW = planner.Built.ContainsKey(new Point(location.X, location.Y + 1));

            // 1 combo of 4
            if (NW && NE && SE && SW)
                return "four_way";

            // four different combos of 3
            else if (NW && NE && SE)
                return "ne_threeway";
            else if (NW && NE && SW)
                return "nw_threeway";
            else if (NW && SE && SW)
                return "sw_threeway";
            else if (NE && SE && SW)
                return "se_threeway";

            // 6 combos of 2
            else if (SW && SE)
                return "sw_se_corner";
            else if (NW && SW)
                return "nw_sw_corner";
            else if (NW && NE)
                return "nw_ne_corner";
            else if (NE && SE)
                return "ne_se_corner";
            else if (SW && NE)
                return "sw_ne_connection";
            else if (NW && SE)
                return "nw_se_connection";

            // 4 combos of 1
            else if (SW)
                return "sw_endpoint";
            else if (NW)
                return "nw_endpoint";
            else if (NE)
                return "ne_endpoint";
            else if (SE)
                return "se_endpoint";

            // 1 cobmo of 0
            else
                return "single";
        }
    }
}
