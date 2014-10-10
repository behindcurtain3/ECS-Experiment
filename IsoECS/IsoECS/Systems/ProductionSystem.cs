using System;
using System.Collections.Generic;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;
using IsoECS.GamePlay;

namespace IsoECS.Systems
{
    public class ProductionSystem : ISystem
    {
        public void Init(List<Entity> entities)
        {

        }

        public void Shutdown(List<Entity> entities)
        {

        }

        public void Update(List<Entity> entities, int dt)
        {
            List<Entity> producers = entities.FindAll(delegate(Entity e) { return e.HasComponent<ProductionComponent>(); });

            foreach (Entity e in producers)
            {
                ProductionComponent p = e.Get<ProductionComponent>();

                if (p.Recipe == null)
                    continue;
            }
        }
    }
}
