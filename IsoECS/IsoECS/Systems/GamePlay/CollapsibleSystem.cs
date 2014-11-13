using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using TecsDotNet;
using TecsDotNet.Managers;

namespace IsoECS.Systems.GamePlay
{
    public class CollapsibleSystem : GameSystem
    {
        List<Entity> collapsibles;

        public override void Init()
        {
            collapsibles = new List<Entity>();
            collapsibles.AddRange(World.Entities.FindAll(delegate(Entity e) { return e.HasComponent<CollapsibleComponent>(); }));

            World.Entities.EntityAdded += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityAdded);
            World.Entities.EntityRemoved += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityRemoved);
            World.Date.HourChanged += new GameDateComponent.GameDateEventHandler(Date_Changed);
        }

        public override void Shutdown()
        {
            World.Date.HourChanged -= Date_Changed;
        }

        public override void Update(double dt)
        {            
        }

        private void Date_Changed(GameDateComponent sender)
        {
            foreach (Entity e in collapsibles)
            {
                CollapsibleComponent c = e.Get<CollapsibleComponent>();

                c.Value--;

                // TODO: collapse the entity if the value is below 0
            }
        }

        private void Entities_EntityRemoved(object sender, EntityEventArgs e)
        {
            collapsibles.Remove(e.Entity);
        }

        private void Entities_EntityAdded(object sender, EntityEventArgs e)
        {
            if (e.Entity.HasComponent<CollapsibleComponent>())
                collapsibles.Add(e.Entity);
        }
    }
}
