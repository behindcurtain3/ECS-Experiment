using System;
using System.Collections.Generic;
using System.Linq;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace IsoECS.Entities
{
    public class EntityManager
    {
        public List<Entity> Entities { get; private set; }
        public Manager UI { get; set; }
        public IsometricMapComponent Map { get; private set; }
        public RoadPlannerComponent Roads { get; private set; }
        public FoundationPlannerComponent Foundations { get; private set; }
        public CollisionMapComponent Collisions { get; private set; }
        public CityInformationComponent CityInformation { get; private set; }
        public GameDateComponent Date { get; set; }

        public EntityManager()
        {
            Entities = new List<Entity>();
        }

        public void RemoveEntity(Entity entity)
        {
            Entities.Remove(entity);

            Point index = entity.Get<PositionComponent>().Index;
            foreach (IsoECS.Components.Component c in entity.Components.Values.ToList())
            {
                switch (c.GetType().Name)
                {
                    case "FoundationComponent":
                        FoundationComponent foundation = (FoundationComponent)c;

                        // remove any foundations from the planner
                        foreach (LocationValue lv in foundation.Plan)
                        {
                            Point update = new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y);
                            if (Foundations.SpaceTaken.ContainsKey(update))
                                Foundations.SpaceTaken.Remove(update);
                        }
                        break;
                }
            }
        }

        public void AddEntity(Entity entity)
        {
            Point index = entity.Get<PositionComponent>().Index;

            Entities.Add(entity);

            // update the game data
            foreach (IsoECS.Components.Component c in entity.Components.Values.ToList())
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

                    case "CityInformationComponent":
                        CityInformation = (CityInformationComponent)c;
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
                            Foundations.SpaceTaken.Add(update, entity.ID);
                        }
                        break;

                    case "FoundationPlannerComponent":
                        Foundations = (FoundationPlannerComponent)c;
                        break;

                    case "GameDateComponent":
                        Date = (GameDateComponent)c;
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

        public List<Point> GetValidExitsFromFoundation(Entity entity)
        {
            // list to hold valid road tiles to move to
            List<Point> validLandings = new List<Point>();
            FoundationComponent foundation = entity.Get<FoundationComponent>();
            PositionComponent position = entity.Get<PositionComponent>();

            foreach (LocationValue plot in foundation.Plan)
            {
                // check the ortho points around the plot position
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if (Math.Abs(x) == Math.Abs(y))
                            continue;

                        Point p = new Point(position.Index.X + plot.Offset.X + x, position.Index.Y + plot.Offset.Y + y);

                        if (!Isometric.ValidIndex(Map, p.X, p.Y))
                            continue;

                        // can only exit onto roads
                        //if (Roads.IsRoadAt(p) && !validLandings.Contains(p))
                        //    validLandings.Add(p);

                        if ((!Collisions.Map.ContainsKey(p) || Collisions.Map[p] != -1) && !validLandings.Contains(p))
                            validLandings.Add(p);
                    }
                }
            }

            return validLandings;
        }

        public List<Entity> GetBuildingsWithinWalkableDistance(int startID, int distance)
        {
            List<Entity> entitiesWithinRange = new List<Entity>();
            Entity startEntity = GetEntity(startID);

            if (startEntity == null)
                return entitiesWithinRange;

            List<Point> exits = GetValidExitsFromFoundation(startEntity);

            foreach (Point exit in exits)
            {
                // walk the "road path" until distance is reached
                List<Point> roads = Pathwalker.GetRoadsWithinDistance(Roads, exit, distance);

                foreach(Point road in roads)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            if (Math.Abs(x) == Math.Abs(y))
                                continue;

                            // check ortho for foundations
                            Point p = new Point(road.X + x, road.Y + y);

                            if (Foundations.SpaceTaken.ContainsKey(p))
                            {
                                Entity e = GetEntity(Foundations.SpaceTaken[p]);
                                if (e != null && e.HasComponent<ProductionComponent>() && !entitiesWithinRange.Contains(e))
                                    entitiesWithinRange.Add(e);
                            }
                        }
                    }
                }
            }            

            return entitiesWithinRange;
        }

        public Entity GetEntity(int id)
        {
            return Entities.Find(delegate(Entity e) { return e.ID == id; });
        }
    }
}