using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using IsoECS.Entities;
using IsoECS.DataStructures;
using IsoECS.Util;

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

        public void UpdateGfx(List<Entity> entities)
        {
            // make sure the road has the right gfx & data
            foreach (Entity entity in entities)
            {
                RoadComponent road = entity.Get<RoadComponent>();
                DrawableComponent drawable = entity.Get<DrawableComponent>();

                // TODO: handle the removal of entity somewhere else
                if (!Built.ContainsKey(road.BuiltAt))
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
                        road.RoadType = Built[road.BuiltAt];
                        drawable.Add("Foundation", Serialization.DeepCopy<IGameDrawable>(DrawableLibrary.Instance.Get(road.RoadType)));
                    }
                }
            }
        }

        public void AddOrUpdate(IsometricMapComponent map, Point location, bool recursion = false)
        {
            string value = "dirt-road-" + GetRoadValue(map, location);

            // set the value of built into the roadPlanner
            if (!Built.ContainsKey(location))
                Built.Add(location, value);
            else
                Built[location] = value;

            if (recursion)
            {
                UpdateNeighbors(map, location);
            }
        }

        public void RemoveRoad(IsometricMapComponent map, Point location)
        {
            if (Built.ContainsKey(location))
                Built.Remove(location);

            UpdateNeighbors(map, location);
        }

        private void UpdateNeighbors(IsometricMapComponent map, Point location)
        {
            // update the neighbors to account for this one
            if (map.IsValidIndex(location.X - 1, location.Y) && Built.ContainsKey(new Point(location.X - 1, location.Y)))
                AddOrUpdate(map, new Point(location.X - 1, location.Y));

            if (map.IsValidIndex(location.X, location.Y - 1) && Built.ContainsKey(new Point(location.X, location.Y - 1)))
                AddOrUpdate(map, new Point(location.X, location.Y - 1));

            if (map.IsValidIndex(location.X + 1, location.Y) && Built.ContainsKey(new Point(location.X + 1, location.Y)))
                AddOrUpdate(map, new Point(location.X + 1, location.Y));

            if (map.IsValidIndex(location.X, location.Y + 1) && Built.ContainsKey(new Point(location.X, location.Y + 1)))
                AddOrUpdate(map, new Point(location.X, location.Y + 1));
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
    }
}
