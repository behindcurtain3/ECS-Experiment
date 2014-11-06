using System.Collections.Generic;
using IsoECS.Behaviors;
using IsoECS.Components.GamePlay;
using TecsDotNet;

namespace IsoECS.Systems.GamePlay
{
    public class BehaviorSystem : GameSystem
    {
        private List<Entity> brains = new List<Entity>();

        public override void Init()
        {
            base.Init();
            brains.AddRange(World.Entities.FindAll(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); }));

            World.Entities.EntityAdded += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityAdded);
            World.Entities.EntityRemoved += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityRemoved);
        }

        public override void Shutdown()
        {
            brains.Clear();

            World.Entities.EntityAdded -= Entities_EntityAdded;
            World.Entities.EntityRemoved -= Entities_EntityRemoved;
        }

        public override void Update(double dt)
        {
            foreach (Entity e in brains)
            {
                CitizenComponent citizen = e.Get<CitizenComponent>();

                if (citizen.Brain == null)
                {
                    citizen.Brain = new DefaultBehavior();
                    citizen.Brain.Init(World, e);
                }

                citizen.Brain.Update(e, dt);
            }
        }


        private void Entities_EntityRemoved(Entity e, World world)
        {
            brains.Remove(e);
        }

        private void Entities_EntityAdded(Entity e, World world)
        {
            if (e.HasComponent<CitizenComponent>())
                brains.Add(e);
        }
    }
}
