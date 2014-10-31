using System.Collections.Generic;
using IsoECS.Behaviors;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;

namespace IsoECS.Systems.GamePlay
{
    public class BehaviorSystem : ISystem
    {
        private List<Entity> brains = new List<Entity>();

        public void Init()
        {
            brains.AddRange(EntityManager.Instance.Entities.FindAll(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); }));

            EntityManager.Instance.EntityAdded += new EntityManager.EntityEventHandler(Instance_EntityAdded);
            EntityManager.Instance.EntityRemoved += new EntityManager.EntityEventHandler(Instance_EntityRemoved);
        }

        public void Shutdown()
        {
            brains.Clear();
        }

        public void Update(int dt)
        {
            foreach (Entity e in brains)
            {
                CitizenComponent citizen = e.Get<CitizenComponent>();

                if (citizen.Brain == null)
                {
                    citizen.Brain = new DefaultBehavior();
                    citizen.Brain.Init(e);
                }

                citizen.Brain.Update(e, dt);
            }
        }

        private void Instance_EntityRemoved(Entity e)
        {
            brains.Remove(e);
        }

        void Instance_EntityAdded(Entity e)
        {
            if (e.HasComponent<CitizenComponent>())
                brains.Add(e);
        }
    }
}
