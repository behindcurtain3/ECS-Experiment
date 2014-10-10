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
        private List<Entity> _entities;

        private EntityLibrary()
        {
            _entities = new List<Entity>();
        }

        private void AddEntity(JObject source)
        {
            Entity e = LoadEntity(source);
            _entities.Add(e);
        }

        public List<Entity> GetAll<T>()
        {
            return _entities.FindAll(delegate(Entity e) { return e.HasComponent<T>(); });   
        }

        public Entity LoadEntity(JObject source)
        {
            Entity e = new Entity();

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

                    case "CollisionComponent":
                        CollisionComponent collision = JsonConvert.DeserializeObject<CollisionComponent>(o.ToString());
                        c = collision;
                        break;

                    case "CollisionMapComponent":
                        CollisionMapComponent collisionMap = JsonConvert.DeserializeObject<CollisionMapComponent>(o.ToString());
                        c = collisionMap;
                        break;

                    case "DrawableComponent":
                        DrawableComponent drawable = JsonConvert.DeserializeObject<DrawableComponent>(o.ToString());

                        // add any sprites to the drawable list
                        drawable.Drawables.AddRange(drawable.Sprites);

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

                    case "IsometricMapComponent":
                        IsometricMapComponent map = JsonConvert.DeserializeObject<IsometricMapComponent>(o.ToString());
                        c = map;
                        break;

                    case "MoveToTargetComponent":
                        MoveToTargetComponent move = JsonConvert.DeserializeObject<MoveToTargetComponent>(o.ToString());
                        c = move;
                        break;

                    case "PositionComponent":
                        PositionComponent position = JsonConvert.DeserializeObject<PositionComponent>(o.ToString());
                        c = position;
                        break;

                    case "RoadComponent":
                        // setup the road component (nothing atm)
                        RoadComponent road = JsonConvert.DeserializeObject<RoadComponent>(o.ToString());
                        c = road;
                        break;

                    case "RoadPlannerComponent":
                        RoadPlannerComponent roadPlanner = JsonConvert.DeserializeObject<RoadPlannerComponent>(o.ToString());
                        c = roadPlanner;
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
