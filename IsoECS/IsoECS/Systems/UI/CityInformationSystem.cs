using System.Collections.Generic;
using System.Linq;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;
using IsoECS.Systems.Threaded;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
using IsoECS.DataRenderers;

namespace IsoECS.Systems.UI
{
    public class CityInformationSystem : ISystem
    {
        private CityInformationRenderer cir;

        public void Init()
        {
            cir = new CityInformationRenderer(EntityManager.Instance.CityInformation, EntityManager.Instance.UI);

            Panel cirp = cir.GetControl(null);

            EntityManager.Instance.EntityAdded += new EntityManager.EntityEventHandler(Instance_EntityAdded);
            EntityManager.Instance.EntityRemoved += new EntityManager.EntityEventHandler(Instance_EntityRemoved);

            int pop = EntityManager.Instance.Entities.Count(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); });

            EntityManager.Instance.CityInformation.Population = pop;
        }

        public void Update(int dt)
        {
        }

        public void Shutdown()
        {
            if(cir != null)
                cir.Shutdown();

            EntityManager.Instance.EntityAdded -= Instance_EntityAdded;
            EntityManager.Instance.EntityRemoved -= Instance_EntityAdded;
        }

        private void Instance_EntityRemoved(Entity e)
        {
            if (e.HasComponent<CitizenComponent>())
                EntityManager.Instance.CityInformation.Population--;
        }

        private void Instance_EntityAdded(Entity e)
        {
            if (e.HasComponent<CitizenComponent>())
                EntityManager.Instance.CityInformation.Population++;
        }
    }
}
