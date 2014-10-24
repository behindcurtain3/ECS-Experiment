using System;
using System.Collections.Generic;
using System.IO;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures.Json;
using IsoECS.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;
using IsoECS.DataStructures.Json.Converters;

namespace IsoECS.DataStructures
{
    public sealed class EntityLibrary
    {
        private static readonly EntityLibrary _instance = new EntityLibrary();

        public static EntityLibrary Instance
        {
            get { return _instance; }
        }

        // stores ready made entities by their unique ID
        private Dictionary<string, Entity> _entities;

        private EntityLibrary()
        {
            _entities = new Dictionary<string, Entity>();
        }

        private void AddEntity(JObject source)
        {
            Entity e = LoadEntity(source);

            if (String.IsNullOrWhiteSpace(e.UniqueID))
            {
                e.UniqueID = e.ID.ToString();
            }

            _entities.Add(e.UniqueID, e);
        }

        public List<Entity> GetAll<T>()
        {
            return new List<Entity>(_entities.Values).FindAll(delegate(Entity e) { return e.HasComponent<T>(); });
        }

        public Entity Get(string uniqueID)
        {
            return _entities[uniqueID];
        }

        public Entity LoadEntity(JObject source)
        {
            Entity e = new Entity();
            e.UniqueID = (string)source["UniqueID"];

            // Get the components from the json
            IEnumerable<JObject> components = source["Components"].ToObject<IEnumerable<JObject>>();

            // Loop through the components and setup the appropriate components 
            foreach (JObject o in components)
            {
                Component c = null;
                JToken tokenName;

                if (!o.TryGetValue("Type", out tokenName))
                {
                    Console.WriteLine("Unable to load component, no type specified.");
                    Console.WriteLine(o.ToString());
                    continue;
                }

                // get the component type
                string typeName = tokenName.ToString();

                // do a custom action based on the type below
                switch (typeName)
                {
                    case "BuildableComponent":
                        BuildableComponent buildable = JsonConvert.DeserializeObject<BuildableComponent>(o.ToString());
                        c = buildable;
                        break;

                    case "CitizenComponent":
                        CitizenComponent citizen = JsonConvert.DeserializeObject<CitizenComponent>(o.ToString());
                        c = citizen;
                        break;

                    case "CityInformationComponent":
                        CityInformationComponent city = JsonConvert.DeserializeObject<CityInformationComponent>(o.ToString());
                        c = city;
                        break;

                    case "CollisionComponent":
                        CollisionComponent collision = JsonConvert.DeserializeObject<CollisionComponent>(o.ToString());
                        c = collision;
                        break;

                    case "CollisionMapComponent":
                        CollisionMapComponent collisionMap = JsonConvert.DeserializeObject<CollisionMapComponent>(o.ToString());
                        c = collisionMap;
                        break;

                    case "DrawableComponent":
                        DrawableComponent drawable = null;
                        if (o["Sources"] != null)
                        {
                            drawable = new DrawableComponent();

                            IEnumerable<string> sources = o["Sources"].Values<string>();
                            foreach (string str in sources)
                            {
                                IGameDrawable d = DrawableLibrary.Instance.Get(str);

                                drawable.Add(d.Layer, d);
                            }

                        }
                        if (o["Drawables"] != null)
                        {
                            DrawableComponent dd = JsonConvert.DeserializeObject<DrawableComponent>(o.ToString(), new DrawableConverter());

                            if (drawable != null)
                            {
                                foreach (KeyValuePair<string, List<IGameDrawable>> kvp in dd.Drawables)
                                {
                                    foreach (IGameDrawable d in kvp.Value)
                                    {
                                        drawable.Add(kvp.Key, d);
                                    }
                                }
                            }
                            else
                            {
                                drawable = dd;
                            }
                        }

                        c = drawable;
                        break;

                    case "FoundationComponent":
                        FoundationComponent floor = JsonConvert.DeserializeObject<FoundationComponent>(o.ToString());

                        switch (floor.PlanType)
                        {
                            case "Normal":
                                // nothing
                                break;
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
                        c = floor;
                        break;

                    case "FoundationPlannerComponent":
                        FoundationPlannerComponent foundationPlanner = JsonConvert.DeserializeObject<FoundationPlannerComponent>(o.ToString());
                        c = foundationPlanner;
                        break;
                        
                    case "GameDateComponent":
                        GameDateComponent date = JsonConvert.DeserializeObject<GameDateComponent>(o.ToString());
                        c = date;
                        break;

                    case "HousingComponent":
                        HousingComponent housing = JsonConvert.DeserializeObject<HousingComponent>(o.ToString());
                        c = housing;
                        break;

                    case "Inventory":
                        Inventory inventory = JsonConvert.DeserializeObject<Inventory>(o.ToString());
                        c = inventory;
                        break;

                    case "IsometricMapComponent":
                        IsometricMapComponent map = JsonConvert.DeserializeObject<IsometricMapComponent>(o.ToString());
                        c = map;
                        break;

                    case "MoveToTargetComponent":
                        MoveToTargetComponent move = JsonConvert.DeserializeObject<MoveToTargetComponent>(o.ToString());
                        c = move;
                        break;

                    case "ProductionComponent":
                        ProductionComponent production = JsonConvert.DeserializeObject<ProductionComponent>(o.ToString());
                        c = production;
                        break;

                    case "PositionComponent":
                        PositionComponent position = JsonConvert.DeserializeObject<PositionComponent>(o.ToString());
                        c = position;
                        break;

                    case "RoadComponent":
                        RoadComponent road = JsonConvert.DeserializeObject<RoadComponent>(o.ToString());
                        c = road;
                        break;

                    case "RoadPlannerComponent":
                        RoadPlannerComponent roadPlanner = JsonConvert.DeserializeObject<RoadPlannerComponent>(o.ToString());
                        c = roadPlanner;
                        break;

                    case "SpawnerComponent":
                        SpawnerComponent spawner = JsonConvert.DeserializeObject<SpawnerComponent>(o.ToString());
                        c = spawner;
                        break;

                    case "StockpileComponent":
                        StockpileComponent stockpile = JsonConvert.DeserializeObject<StockpileComponent>(o.ToString());
                        c = stockpile;
                        break;
                }

                // add the component to the entity
                if (c != null)
                    e.AddComponent(c);
            }
            return e;
        }

        public void LoadFromJson(string path, bool config = false)
        {
            string json = File.ReadAllText(path);

            if (config)
            {
                JsonListConfig jsonConfig = JsonConvert.DeserializeObject<JsonListConfig>(json);
                foreach (string str in jsonConfig.List)
                {
                    LoadFromJson(str);
                }
            }
            else
            {
                JObject file = JObject.Parse(json);
                foreach (JObject o in file["Entities"].ToObject<IEnumerable<JObject>>())
                {
                    AddEntity(o);
                }
            }
        }
    }
}
