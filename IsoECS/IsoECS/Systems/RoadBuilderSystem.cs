using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Systems
{
    public class RoadBuilderSystem : ISystem
    {
        private IsometricMapComponent Map { get; set; }

        public void Update(List<Entity> entities, int dt)
        {
            Entity inputEntity = entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });
            RoadplannerComponent roadPlanner = entities.Find(delegate(Entity e) { return e.HasComponent<RoadplannerComponent>(); }).Get<RoadplannerComponent>();

            if (inputEntity != null)
            {
                MouseState currentMouse = inputEntity.Get<InputController>().CurrentMouse;
                MouseState prevMouse = inputEntity.Get<InputController>().PrevMouse;

                // click registered
                if (currentMouse.LeftButton == ButtonState.Pressed || currentMouse.RightButton == ButtonState.Pressed)
                {
                    Entity cameraEntity = entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
                    Entity mapEntity = entities.Find(delegate(Entity e) { return e.HasComponent<IsometricMapComponent>(); });
                    
                    PositionComponent camera = cameraEntity.Get<PositionComponent>();
                    Map = mapEntity.Get<IsometricMapComponent>();

                    // set the starting coords
                    int x = currentMouse.X + (int)camera.X;
                    int y = currentMouse.Y + (int)camera.Y;

                    // pick out the tile index that the screen coords intersect
                    Point index = Isometric.GetPointAtScreenCoords(Map, x, y);

                    // don't built if invalid index
                    if (!Isometric.ValidIndex(Map, index.X, index.Y))
                        return;

                    // update the planner
                    if (currentMouse.LeftButton == ButtonState.Pressed)
                    {
                        // check to see if another road already exists there
                        // already built
                        if (roadPlanner.Built.ContainsKey(index))
                            return;

                        AddOrUpdateRoad(roadPlanner, index, true);

                        // translate the index into a screen position
                        Vector2 position = Isometric.GetIsometricPosition(Map, 0, index.Y, index.X);

                        Entity roadEntity = new Entity();
                        roadEntity.AddComponent(new PositionComponent(position));
                        roadEntity.AddComponent(new RoadComponent()
                        {
                            RoadType = roadPlanner.Built[index],
                            BuiltAt = index
                        });
                        roadEntity.AddComponent(new DrawableComponent()
                        {
                            Texture = Textures.Instance.Get("isometric_roads"),
                            Source = Textures.Instance.GetSource("isometric_roads", roadPlanner.Built[index]),
                            Layer = 89
                        });

                        entities.Add(roadEntity);
                    }
                    else
                    {
                        RemoveRoad(roadPlanner, index);
                    }
                    

                    List<Entity> roadEntities = entities.FindAll(delegate(Entity e) { return e.HasComponent<RoadComponent>(); });

                    // make sure the road has the right gfx & data
                    foreach (Entity entity in roadEntities)
                    {
                        RoadComponent road = entity.Get<RoadComponent>();
                        DrawableComponent drawable = entity.Get<DrawableComponent>();

                        if (!roadPlanner.Built.ContainsKey(road.BuiltAt))
                        {
                            // remove the entity
                            entities.Remove(entity);
                        }
                        else
                        {
                            road.RoadType = roadPlanner.Built[road.BuiltAt];
                            drawable.Source = Textures.Instance.GetSource("isometric_roads", road.RoadType);
                        }
                    }
                }
            }
        }

        private void AddOrUpdateRoad(RoadplannerComponent planner, Point location, bool recursion = false)
        {
            string value = GetRoadValue(planner, location);

            // set the value of built into the roadPlanner
            if (!planner.Built.ContainsKey(location))
                planner.Built.Add(location, value);
            else
                planner.Built[location] = value;

            if (recursion)
            {
                UpdateNeighbors(planner, location);
            }
        }

        private void RemoveRoad(RoadplannerComponent planner, Point location)
        {
            if (planner.Built.ContainsKey(location))
                planner.Built.Remove(location);

            UpdateNeighbors(planner, location);
        }

        public void UpdateNeighbors(RoadplannerComponent planner, Point location)
        {
            // update the neighbors to account for this one
            if (Isometric.ValidIndex(Map, location.X - 1, location.Y) && planner.Built.ContainsKey(new Point(location.X - 1, location.Y)))
                AddOrUpdateRoad(planner, new Point(location.X - 1, location.Y));

            if (Isometric.ValidIndex(Map, location.X, location.Y - 1) && planner.Built.ContainsKey(new Point(location.X, location.Y - 1)))
                AddOrUpdateRoad(planner, new Point(location.X, location.Y - 1));

            if (Isometric.ValidIndex(Map, location.X + 1, location.Y) && planner.Built.ContainsKey(new Point(location.X + 1, location.Y)))
                AddOrUpdateRoad(planner, new Point(location.X + 1, location.Y));

            if (Isometric.ValidIndex(Map, location.X, location.Y + 1) && planner.Built.ContainsKey(new Point(location.X, location.Y + 1)))
                AddOrUpdateRoad(planner, new Point(location.X, location.Y + 1));
        }

        private string GetRoadValue(RoadplannerComponent planner, Point location)
        {
            // set the value of the location to index of the type of road at this location
            bool NW = false;
            bool NE = false;
            bool SE = false;
            bool SW = false;

            if (Isometric.ValidIndex(Map, location.X - 1, location.Y))
                NW = planner.Built.ContainsKey(new Point(location.X - 1, location.Y));

            if (Isometric.ValidIndex(Map, location.X, location.Y - 1))
                NE = planner.Built.ContainsKey(new Point(location.X, location.Y - 1));

            if (Isometric.ValidIndex(Map, location.X + 1, location.Y))
                SE = planner.Built.ContainsKey(new Point(location.X + 1, location.Y));

            if (Isometric.ValidIndex(Map, location.X, location.Y + 1))
                SW = planner.Built.ContainsKey(new Point(location.X, location.Y + 1));

            // 1 combo of 4
            if(NW && NE && SE && SW)
                return "four_way";

            // four different combos of 3
            else if(NW && NE && SE)
                return "ne_threeway";
            else if(NW && NE && SW)
                return "nw_threeway";
            else if(NW && SE && SW)
                return "sw_threeway";
            else if(NE && SE && SW)
                return "se_threeway";

            // 6 combos of 2
            else if(SW && SE)
                return "sw_se_corner";
            else if(NW && SW)
                return "nw_sw_corner";
            else if(NW && NE)
                return "nw_ne_corner";
            else if(NE && SE)
                return "ne_se_corner";
            else if(SW && NE)
                return "sw_ne_connection";
            else if(NW && SE)
                return "nw_se_connection";

            // 4 combos of 1
            else if(SW)
                return "sw_endpoint";
            else if(NW)
                return "nw_endpoint";
            else if(NE)
                return "ne_endpoint";
            else if(SE)
                return "se_endpoint";

            // 1 cobmo of 0
            else
                return "single";
        }

        private void GetRoadType(RoadplannerComponent planner, int x, int y)
        {

        }
    }
}
