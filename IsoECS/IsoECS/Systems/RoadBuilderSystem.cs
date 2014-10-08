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
            Entity dataTracker = entities.Find(delegate(Entity e) { return e.HasComponent<RoadplannerComponent>(); });
            RoadplannerComponent roadPlanner = dataTracker.Get<RoadplannerComponent>();
            CollisionMapComponent collisionMap = dataTracker.Get<CollisionMapComponent>();


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

                        RoadsHelper.AddOrUpdateRoad(roadPlanner, Map, index, true);

                        // update the collision map
                        collisionMap.Collision[index] = 8;

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
                        RoadsHelper.RemoveRoad(roadPlanner, Map, index);

                        collisionMap.Collision[index] = 64;
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

        
    }
}
