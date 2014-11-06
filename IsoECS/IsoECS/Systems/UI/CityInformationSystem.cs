using System.Linq;
using IsoECS.Components.GamePlay;
using IsoECS.DataRenderers;
using TecsDotNet;
using TomShane.Neoforce.Controls;

namespace IsoECS.Systems.UI
{
    public class CityInformationSystem : GameSystem
    {
        private CityInformationRenderer cir;

        public override void Init()
        {
            cir = new CityInformationRenderer(World.City, World);

            Panel cirp = cir.GetControl(null);

            World.Entities.EntityAdded += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityAdded);
            World.Entities.EntityRemoved += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityRemoved);

            int pop = World.Entities.Count(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); });

            World.City.Population = pop;
        }

        private void Entities_EntityRemoved(Entity e, World world)
        {
            if (e.HasComponent<CitizenComponent>())
                World.City.Population--;
        }

        private void Entities_EntityAdded(Entity e, World world)
        {
            if (e.HasComponent<CitizenComponent>())
                World.City.Population++;
        }

        public override void Update(double dt)
        {
        }

        public override void Shutdown()
        {
            if(cir != null)
                cir.Shutdown();

            World.Entities.EntityAdded -= Entities_EntityAdded;
            World.Entities.EntityRemoved -= Entities_EntityRemoved;
        }
    }
}
