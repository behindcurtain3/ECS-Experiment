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
                        foreach (IGameDrawable sprite in drawable.Drawables["Foundation"].ToList())
                        {
                            if (sprite.UniqueID.Contains("road"))
                            {
                                // remove old road drawables
                                drawable.Drawables["Foundation"].Remove(sprite);
                            }
                        }
                        road.RoadType = planner.Built[road.BuiltAt];
                        drawable.Add("Foundation", Serialization.DeepCopy<IGameDrawable>(DrawableLibrary.Instance.Get(road.RoadType)));
                    }
                }
            }
        }

        public static void AddOrUpdateRoad(RoadPlannerComponent planner, IsometricMapComponent map, Point location, bool recursion = false)
        {
            string value = "dirt-road-" + GetRoadValue(planner, map, location);

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
            if (map.IsValidIndex(location.X - 1, location.Y) && planner.Built.ContainsKey(new Point(location.X - 1, location.Y)))
                AddOrUpdateRoad(planner, map, new Point(location.X - 1, location.Y));

            if (map.IsValidIndex(location.X, location.Y - 1) && planner.Built.ContainsKey(new Point(location.X, location.Y - 1)))
                AddOrUpdateRoad(planner, map, new Point(location.X, location.Y - 1));

            if (map.IsValidIndex(location.X + 1, location.Y) && planner.Built.ContainsKey(new Point(location.X + 1, location.Y)))
                AddOrUpdateRoad(planner, map, new Point(location.X + 1, location.Y));

            if (map.IsValidIndex(location.X, location.Y + 1) && planner.Built.ContainsKey(new Point(location.X, location.Y + 1)))
                AddOrUpdateRoad(planner, map, new Point(location.X, location.Y + 1));
        }

        private static string GetRoadValue(RoadPlannerComponent planner, IsometricMapComponent map, Point location)
        {
            // set the value of the location to index of the type of road at this location
            bool NW = false;
            bool NE = false;
            bool SE = false;
            bool SW = false;

            if (map.IsValidIndex(location.X - 1, location.Y))
                NW = planner.Built.ContainsKey(new Point(location.X - 1, location.Y));

            if (map.IsValidIndex(location.X, location.Y - 1))
                NE = planner.Built.ContainsKey(new Point(location.X, location.Y - 1));

            if (map.IsValidIndex(location.X + 1, location.Y))
                SE = planner.Built.ContainsKey(new Point(location.X + 1, location.Y));

            if (map.IsValidIndex(location.X, location.Y + 1))
                SW = planner.Built.ContainsKey(new Point(location.X, location.Y + 1));

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
    }
}
