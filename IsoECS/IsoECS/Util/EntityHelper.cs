using System;
using System.Collections.Generic;
using System.Linq;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Entities;
using Microsoft.Xna.Framework;

namespace IsoECS.Util
{
    public class EntityHelper
    {

        public static void ActivateEntity(List<Entity> entities, Entity entity)
        {
            Entity mapEntity = entities.Find(delegate(Entity e) { return e.HasComponent<IsometricMapComponent>(); });
            Entity dataTracker = entities.Find(delegate(Entity e) { return e.HasComponent<RoadPlannerComponent>(); });

            IsometricMapComponent map = (mapEntity == null) ? null : mapEntity.Get<IsometricMapComponent>();
            RoadPlannerComponent roadPlanner = (dataTracker == null) ? null : dataTracker.Get<RoadPlannerComponent>();;
            CollisionMapComponent collisionMap = (dataTracker == null) ? null : dataTracker.Get<CollisionMapComponent>();
            FoundationPlannerComponent foundationPlanner = (dataTracker == null) ? null : dataTracker.Get<FoundationPlannerComponent>();
            Point index = entity.Get<PositionComponent>().Index;

            entities.Add(entity);

            // update the game data
            foreach (Component c in entity.Components.Values.ToList())
            {
                switch (c.GetType().Name)
                {
                    case "CitizenComponent":
                        CitizenComponent citizen = (CitizenComponent)c;

                        // fill in the data if it doesn't already exist
                        if(String.IsNullOrWhiteSpace(citizen.Name))
                        {
                            // generate name
                            string[] names = {"Steve", "John", "Bill" };
                            citizen.Name = names[Game1.Random.Next(0, names.Length)];
                        }

                        if(String.IsNullOrWhiteSpace(citizen.FamilyName))
                        {
                            // generate family name
                            string[] names = { "Johnson", "Miller", "Smith" };
                            citizen.FamilyName = names[Game1.Random.Next(0, names.Length)];
                        }

                        if (citizen.Age == 0)
                        {
                            // generate age
                            citizen.Age = Game1.Random.Next(14, 46);
                        }

                        if (citizen.Money == 0)
                        {
                            citizen.Money = (int)(Game1.Random.NextDouble() * 100);
                        }
                        break;

                    case "CollisionComponent":
                        CollisionComponent collision = (CollisionComponent)c;

                        foreach (LocationValue lv in collision.Plan)
                        {
                            Point p = new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y);

                            if (collisionMap.Collision.ContainsKey(p))
                                collisionMap.Collision[p] = lv.Value;
                            else
                                collisionMap.Collision.Add(p, lv.Value);
                        }
                        break;

                    case "FoundationComponent":
                        FoundationComponent floor = (FoundationComponent)c;

                        // update the floor planner
                        foreach (LocationValue lv in floor.Plan)
                        {
                            Point update = new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y);
                            foundationPlanner.SpaceTaken.Add(update, true);
                        }
                        break;

                    case "IsometricMapComponent":
                        map = entity.Get<IsometricMapComponent>();

                        if (map.Terrain == null)
                        {
                            map = Isometric.CreateMap(map.SpriteSheetName, map.TxWidth, map.TxHeight, map.PxTileWidth, map.PxTileHeight);

                            // replace the map
                            entity.RemoveComponent(entity.Get<IsometricMapComponent>());
                            entity.AddComponent(map);
                        }
                        break;

                    case "PositionComponent":
                        PositionComponent position = (PositionComponent)c;

                        if(!String.IsNullOrWhiteSpace(position.GenerateAt))
                        {
                            int xIndex = -1;
                            int yIndex = -1;

                            switch (position.GenerateAt)
                            {
                                // random
                                case "Edge":
                                    int side = Game1.Random.Next(4);
                        
                                    switch (side)
                                    {
                                        case 0:
                                            // northwest
                                            xIndex = 0;
                                            yIndex = Game1.Random.Next(1, map.TxHeight);
                                            break;
                                        case 1:
                                            // northeast
                                            xIndex = Game1.Random.Next(1, map.TxWidth);
                                            yIndex = 0;
                                            break;
                                        case 2:
                                            // southeast
                                            xIndex = map.TxWidth - 1;
                                            yIndex = Game1.Random.Next(1, map.TxHeight);
                                            break;
                                        default:
                                            // southwest
                                            xIndex = Game1.Random.Next(1, map.TxWidth);
                                            yIndex = map.TxHeight - 1;
                                            break;
                                    }
                                    break;
                                case "NoEdge":
                                    xIndex = Game1.Random.Next(1, map.TxWidth - 1);
                                    yIndex = Game1.Random.Next(1, map.TxHeight - 1);
                                    break;
                                default:
                                    xIndex = Game1.Random.Next(0, map.TxWidth);
                                    yIndex = Game1.Random.Next(0, map.TxHeight);
                                    break;
                            }

                            Vector2 pos = Isometric.GetIsometricPosition(map, 0, yIndex, xIndex);

                            position.X = pos.X;
                            position.Y = pos.Y;
                            position.Index = new Point(xIndex, yIndex);
                            position.GenerateAt = String.Empty;
                            index = position.Index;
                        }
                        break;

                    case "RoadComponent":
                        // setup the road component
                        RoadComponent road = (RoadComponent)c;
                        road.BuiltAt = index;

                        // update the roads
                        RoadsHelper.AddOrUpdateRoad(roadPlanner, map, index, true);

                        // update the other roads
                        List<Entity> roadEntities = entities.FindAll(delegate(Entity e) { return e.HasComponent<RoadComponent>(); });
                        RoadsHelper.UpdateRoadsGfx(roadEntities, roadPlanner);
                        break;

                    case "SpawnerComponent":
                        break;
                }
            }
        }
    }
}
