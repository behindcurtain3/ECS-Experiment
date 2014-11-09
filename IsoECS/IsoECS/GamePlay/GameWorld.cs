using System;
using System.Collections.Generic;
using System.Linq;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.RenderSystems;
using Microsoft.Xna.Framework;
using TecsDotNet;
using TomShane.Neoforce.Controls;
using Component = TecsDotNet.Component;
using IsoECS.Input;

namespace IsoECS.GamePlay
{
    public class GameWorld : World
    {
        #region Properties

        public RenderSystem Renderer { get; set; }
        public Random Random { get; private set; }

        public Manager UI { get; set; }
        public InputController Input { get; set; }

        public City City { get; set; }
        public IsometricMapComponent Map { get; private set; }
        public RoadPlannerComponent Roads { get; private set; }
        public FoundationPlannerComponent Foundations { get; private set; }
        public CollisionMapComponent Collisions { get; private set; }
        public GameDateComponent Date { get; set; }

        #endregion

        public GameWorld()
            : base()
        {
            Input = new InputController();
            Random = new System.Random();

            Entities.EntityAdded += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityAdded);
            Entities.EntityRemoved += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityRemoved);
        }

        private void Entities_EntityRemoved(Entity e, World world)
        {
        }

        private void Entities_EntityAdded(Entity e, World world)
        {
            Point index = e.Get<PositionComponent>().Index;

            // update the game data
            foreach (Component c in e.Components.Values.ToList())
            {
                switch (c.GetType().Name)
                {
                    case "CitizenComponent":
                        CitizenComponent citizen = (CitizenComponent)c;

                        // fill in the data if it doesn't already exist
                        if (string.IsNullOrWhiteSpace(citizen.Name))
                        {
                            // generate name
                            string[] names = { "Steve", "John", "Bill" };
                            citizen.Name = names[Random.Next(0, names.Length)];
                        }

                        if (string.IsNullOrWhiteSpace(citizen.Surname))
                        {
                            // generate family name
                            string[] names = { "Johnson", "Miller", "Smith" };
                            citizen.Surname = names[Random.Next(0, names.Length)];
                        }

                        if (citizen.Gender == Gender.BOTH)
                        {
                            citizen.Gender = (Gender)Random.Next(1, 3);
                        }

                        if (citizen.Age == 0)
                        {
                            // generate age
                            citizen.Age = Random.Next(14, 46);
                        }

                        if (citizen.Money == 0)
                        {
                            citizen.Money = Random.Next(20, 100);
                        }
                        break;

                    case "CollisionComponent":
                        CollisionComponent collision = (CollisionComponent)c;

                        foreach (LocationValue lv in collision.Plan)
                        {
                            Point p = new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y);

                            if (Collisions.Map.ContainsKey(p))
                                Collisions.Map[p] = (PathTypes)lv.Value;
                            else
                                Collisions.Map.Add(p, (PathTypes)lv.Value);
                        }
                        break;

                    case "CollisionMapComponent":
                        Collisions = (CollisionMapComponent)c;
                        break;

                    case "FoundationComponent":
                        FoundationComponent floor = (FoundationComponent)c;

                        switch (floor.PlanType)
                        {
                            case "Fill":
                                Point start = floor.Plan[0].Offset;
                                Point end = floor.Plan[1].Offset;

                                floor.Plan.Clear(); // clear the plan, the for loops will fill it
                                for (int xx = start.X; xx <= end.X; xx++)
                                {
                                    for (int yy = start.Y; yy <= end.Y; yy++)
                                    {
                                        floor.Plan.Add(new LocationValue() { Offset = new Point(xx, yy) });
                                    }
                                }
                                break;
                        } 

                        // update the floor planner
                        foreach (LocationValue lv in floor.Plan)
                        {
                            Point update = new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y);
                            Foundations.SpaceTaken.Add(update, e.ID);
                        }
                        break;

                    case "FoundationPlannerComponent":
                        Foundations = (FoundationPlannerComponent)c;
                        break;

                    case "GameDateComponent":
                        Date = (GameDateComponent)c;
                        break;

                    case "IsometricMapComponent":
                        Map = e.Get<IsometricMapComponent>();

                        if (Map.Terrain == null)
                        {
                            Map.CreateMap(Map.SpriteSheetName, Map.TxWidth, Map.TxHeight, Map.PxTileWidth, Map.PxTileHeight);

                            // replace the map
                            e.RemoveComponent(e.Get<IsometricMapComponent>());
                            e.AddComponent(Map);
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
                                    int side = Random.Next(4);

                                    switch (side)
                                    {
                                        case 0:
                                            // northwest
                                            xIndex = 0;
                                            yIndex = Random.Next(1, Map.TxHeight);
                                            break;
                                        case 1:
                                            // northeast
                                            xIndex = Random.Next(1, Map.TxWidth);
                                            yIndex = 0;
                                            break;
                                        case 2:
                                            // southeast
                                            xIndex = Map.TxWidth - 1;
                                            yIndex = Random.Next(1, Map.TxHeight);
                                            break;
                                        default:
                                            // southwest
                                            xIndex = Random.Next(1, Map.TxWidth);
                                            yIndex = Map.TxHeight - 1;
                                            break;
                                    }
                                    break;
                                case "NoEdge":
                                    xIndex = Random.Next(1, Map.TxWidth - 1);
                                    yIndex = Random.Next(1, Map.TxHeight - 1);
                                    break;
                                default:
                                    xIndex = Random.Next(0, Map.TxWidth);
                                    yIndex = Random.Next(0, Map.TxHeight);
                                    break;
                            }

                            Vector2 pos = Map.GetPositionFromIndex(xIndex, yIndex);

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

                        // update the planner
                        Roads.AddOrUpdate(this, e, true);
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

                        if (!Map.IsValidIndex(p.X, p.Y))
                            continue;

                        // can only exit onto roads
                        if ((!Collisions.Map.ContainsKey(p) || Collisions.Map[p] == PathTypes.ROAD) && !validLandings.Contains(p))
                            validLandings.Add(p);
                    }
                }
            }

            return validLandings;
        }

        public List<Entity> GetBuildingsWithinWalkableDistance<T>(uint startID, int distance)
        {
            List<Entity> entitiesWithinRange = new List<Entity>();
            Entity startEntity = Entities.Get(startID);

            if (startEntity == null)
                return entitiesWithinRange;

            List<Point> exits = GetValidExitsFromFoundation(startEntity);

            foreach (Point exit in exits)
            {
                // walk the "road path" until distance is reached
                List<Point> roads = Pathwalker.GetRoadsWithinDistance(Roads, exit, distance);

                foreach (Point road in roads)
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
                                Entity e = Entities.Get(Foundations.SpaceTaken[p]);
                                if (e != null && e.HasComponent<T>() && !entitiesWithinRange.Contains(e))
                                    entitiesWithinRange.Add(e);
                            }
                        }
                    }
                }
            }

            return entitiesWithinRange;
        }
    }
}
