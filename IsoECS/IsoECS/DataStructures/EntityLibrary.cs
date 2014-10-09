using System;
using System.Collections.Generic;
using System.IO;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures.Json;
using IsoECS.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            // Get the components from the json
            IEnumerable<JObject> components = source["Components"].ToObject<IEnumerable<JObject>>();

            Entity e = new Entity();

            // Loop through the components and setup the appropriate components 
            foreach(JObject o in components)
            {
                JToken tokenName;
                if (!o.TryGetValue("Type", out tokenName))
                {
                    Console.WriteLine("Unable to load component, no type specified.");
                    Console.WriteLine(o.ToString());
                    continue;
                }

                // get the component type
                string typeName = tokenName.ToString();

                // instantiate the component
                Component c;
                try
                {
                    Type type = Type.GetType("IsoECS.Components.GamePlay." + typeName);
                    c = (Component)Activator.CreateInstance(type);
                }
                catch(ArgumentNullException ex)
                {
                    Console.WriteLine("Unable to load component: " + typeName);
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(o.ToString());
                    continue;
                }

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

                    case "FoundationComponent":
                        FoundationComponent floor = JsonConvert.DeserializeObject<FoundationComponent>(o.ToString());
                        c = floor;
                        break;

                    case "RoadComponent":
                        // setup the road component (nothing atm)
                        RoadComponent road = (RoadComponent)c;
                        break;
                }

                // add the component to the entity
                e.AddComponent(c);
            }

            _entities.Add(e);
        }

        public List<Entity> GetAll<T>()
        {
            return _entities.FindAll(delegate(Entity e) { return e.HasComponent<T>(); });   
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
