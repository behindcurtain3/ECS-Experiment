using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using IsoECS.Components;
using IsoECS.GamePlay;
using Newtonsoft.Json;

namespace IsoECS.Entities
{
    /// <summary>
    /// Notes: use the entity ID for all references to an entity
    /// </summary>
    [Serializable]
    public class Entity : IUnique
    {
        public static uint ID_COUNTER = 0;

        public string UniqueID { get; set; }
        public int ID { get; private set; }
        public Dictionary<Type, Component> Components { get; private set; }

        public Entity()
        {
            ID = (int)++ID_COUNTER;
            Components = new Dictionary<Type, Component>();
        }

        public void AddComponent(Component component)
        {
            Components.Add(component.GetType(), component);
        }

        public bool RemoveComponent(Component component)
        {
            return Components.Remove(component.GetType());
        }

        /// <summary>
        /// Get the typed component requested
        /// </summary>
        /// <typeparam name="T">the type of component requested</typeparam>
        /// <returns>The component with the correct type</returns>
        public T Get<T>()
        {
            return (T)(object)Components[typeof(T)];
        }

        /// <summary>
        /// Checks if the entity has a specific component
        /// </summary>
        /// <typeparam name="T">the type of component to check</typeparam>
        /// <returns>true if entity has the component, false otherwise</returns>
        public bool HasComponent<T>()
        {
            return Components.ContainsKey(typeof(T));
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public Entity DeepCopy()
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, this);
            m.Position = 0;

            Entity e = (Entity)b.Deserialize(m);
            e.ResetID();
            return e;
        }

        private void ResetID()
        {
            ID = (int)++ID_COUNTER;
        }
    }
}
