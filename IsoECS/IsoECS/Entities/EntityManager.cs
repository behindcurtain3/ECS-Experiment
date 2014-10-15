using System.Collections.Generic;

namespace IsoECS.Entities
{
    public class EntityManager
    {
        public List<Entity> Entities { get { return _entities; } }
        private List<Entity> _entities;

        public EntityManager()
        {
            _entities = new List<Entity>();
        }

        public void AddEntity(Entity e)
        {
            _entities.Add(e);
        }

        public void RemoveEntity(Entity e)
        {
            _entities.Remove(e);
        }
    }
}
