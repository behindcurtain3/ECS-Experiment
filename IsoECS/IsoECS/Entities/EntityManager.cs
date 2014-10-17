using System;
using System.Collections.Generic;
using System.Linq;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Util;
using Microsoft.Xna.Framework;

namespace IsoECS.Entities
{
    public class EntityManager
    {
        public List<Entity> Entities { get; private set; }
        public IsometricMapComponent Map { get; private set; }
        public RoadPlannerComponent Roads { get; private set; }
        public FoundationPlannerComponent Foundations { get; private set; }
        public CollisionMapComponent Collisions { get; private set; }

        public EntityManager()
        {
            Entities = new List<Entity>();
        }

        public void RemoveEntity(Entity e)
        {
            Entities.Remove(e);
        }

        public void AddEntity(Entity entity)
        {
            Point index = entity.Get<PositionComponent>().Index;

            Entities.Add(entity);

            // update the game data
            foreach (Component c in entity.Components.Values.ToList())
            {
                switch (c.GetType().Name)
                {
                    case "CitizenComponent":
                        CitizenComponent citizen = (CitizenComponent)c;

                        // fill in the data if it doesn't already exist
                        if (String.IsNullOrWhiteSpace(citizen.Name))
                        {
                            // generate name
                            string[] names = { "Steve", "John", "Bill" };
                            citizen.Name = names[Game1.Random.Next(0, names.Length)];
                        }

                        if (String.IsNullOrWhiteSpace(citizen.FamilyName))
                        {
                            // generate family name
                            string[] names = { "Johnson", "Miller", "Smith" };
                            citizen.FamilyName = names[Game1.Random.Next(0, names.Length)];
                        }

                        if (citizen.Gender == Gender.BOTH)
                        {
                            citizen.Gender = (Gender)Game1.Random.Next(1, 3);
                        }

                        if (citizen.Age == 0)
                        {
                            // generate age
                            citizen.Age = Game1.Random.Next(14, 46);
                        }

                        if (citizen.Money == 0)
                        {
                            citizen.Money = Game1.Random.Next(20, 100);
                        }
                        break;

                    case "CollisionComponent":
                        CollisionComponent collision = (CollisionComponent)c;

                        foreach (LocationValue lv in collision.Plan)
                        {
                            Point p = new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y);

                            if (Collisions.Map.ContainsKey(p))
                                Collisions.Map[p] = lv.Value;
                            else
                                Collisions.Map.Add(p, lv.Value);
                        }
                        break;

                    case "CollisionMapComponent":
                        Collisions = (CollisionMapComponent)c;
                        break;

                    case "FoundationComponent":
                        FoundationComponent floor = (FoundationComponent)c;

                        // update the floor planner
                        foreach (LocationValue lv in floor.Plan)
                        {
                            Point update = new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y);
                            Foundations.SpaceTaken.Add(update, true);
                        }
                        break;

                    case "FoundationPlannerComponent":
                        Foundations = (FoundationPlannerComponent)c;
                        break;

                    case "IsometricMapComponent":
                        Map = entity.Get<IsometricMapComponent>();

                        if (Map.Terrain == null)
                        {
                            Map = Isometric.CreateMap(Map.SpriteSheetName, Map.TxWidth, Map.TxHeight, Map.PxTileWidth, Map.PxTileHeight);

                            // replace the map
                            entity.RemoveComponent(entity.Get<IsometricMapComponent>());
                            entity.AddComponent(Map);
                        }
                        break;

                    case "PositionComponent":
                        PositionComponent position = (PositionComponent)c;

                        if (!String.IsNullOrWhiteSpace(position.GenerateAt))
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
                                            yIndex = Game1.Random.Next(1, Map.TxHeight);
                                            break;
                                        case 1:
                                            // northeast
                                            xIndex = Game1.Random.Next(1, Map.TxWidth);
                                            yIndex = 0;
                                            break;
                                        case 2:
                                            // southeast
                                            xIndex = Map.TxWidth - 1;
                                            yIndex = Game1.Random.Next(1, Map.TxHeight);
                                            break;
                                        default:
                                            // southwest
                                            xIndex = Game1.Random.Next(1, Map.TxWidth);
                                            yIndex = Map.TxHeight - 1;
                                            break;
                                    }
                                    break;
                                case "NoEdge":
                                    xIndex = Game1.Random.Next(1, Map.TxWidth - 1);
                                    yIndex = Game1.Random.Next(1, Map.TxHeight - 1);
                                    break;
                                default:
                                    xIndex = Game1.Random.Next(0, Map.TxWidth);
                                    yIndex = Game1.Random.Next(0, Map.TxHeight);
                                    break;
                            }

                            Vector2 pos = Isometric.GetIsometricPosition(Map, 0, yIndex, xIndex);

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
                        RoadsHelper.AddOrUpdateRoad(Roads, Map, index, true);

                        // update the other roads
                        List<Entity> roadEntities = Entities.FindAll(delegate(Entity e) { return e.HasComponent<RoadComponent>(); });
                        RoadsHelper.UpdateRoadsGfx(roadEntities, Roads);
                        break;

                    case "RoadPlannerComponent":
                        Roads = (RoadPlannerComponent)c;
                        break;

                    case "SpawnerComponent":
                        break;
                }
            }
        }
    }
}
