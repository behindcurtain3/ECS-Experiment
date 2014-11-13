using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using TecsDotNet;
using TecsDotNet.Managers;
using TecsDotNet.Util;

namespace IsoECS.Systems.GamePlay
{
    public class HousingUpgradeSystem : GameSystem
    {
        private List<Entity> housingUnits = new List<Entity>();

        public override void Update(double dt)
        {
        }

        public override void Init()
        {
            base.Init();
            housingUnits.AddRange(World.Entities.FindAll(delegate(Entity e) { return e.HasComponent<HousingComponent>(); }));

            World.Entities.EntityAdded += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityAdded);
            World.Entities.EntityRemoved += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityRemoved);
        }

        private void Entities_EntityRemoved(object sender, EntityEventArgs e)
        {
            if (e.Entity.HasComponent<HousingComponent>())
                e.Entity.Get<HousingComponent>().TennantAdded -= Housing_TennantAdded;

            housingUnits.Remove(e.Entity);
        }

        private void Entities_EntityAdded(object sender, EntityEventArgs e)
        {
            if (e.Entity.HasComponent<HousingComponent>())
            {
                housingUnits.Add(e.Entity);

                HousingComponent housing = e.Entity.Get<HousingComponent>();
                housing.TennantAdded += new HousingComponent.HousingEventHandler(Housing_TennantAdded);
            }
        }

        private void Housing_TennantAdded(HousingComponent sender)
        {
            HousingComponent housing = sender;

            if (string.IsNullOrWhiteSpace(housing.UpgradesTo) || housing.UpgradeRequirements == null)
                return;

            // check each requirement to see if it has been set
            // then check if the requirement fails, if it does just continue to the next entity
            if (housing.UpgradeRequirements.MinimumOccupants != -1)
            {
                if (housing.Tennants.Length < housing.UpgradeRequirements.MinimumOccupants)
                {
                    return;
                }
            }

            Entity entity = World.Entities.Find(delegate(Entity e) { return e.HasComponent<HousingComponent>() && e.Get<HousingComponent>().Equals(housing); });

            // swap out the HousingComponent with the upgraded version and the DrawableComponent
            entity.RemoveComponent(entity.Get<DrawableComponent>());
            entity.RemoveComponent(entity.Get<BuildableComponent>());

            // copy out the entity to upgrade to from the library
            Entity upgradedEntity = (Entity)World.Prototypes[housing.UpgradesTo];

            // copy in the new components
            entity.AddComponent(Serialization.DeepCopy<DrawableComponent>(upgradedEntity.Get<DrawableComponent>()));
            entity.AddComponent(Serialization.DeepCopy<BuildableComponent>(upgradedEntity.Get<BuildableComponent>()));

            // make sure the housing tennant data is copied over
            HousingComponent replacement = Serialization.DeepCopy<HousingComponent>(upgradedEntity.Get<HousingComponent>());

            // copy over housing data
            housing.MaxOccupants = replacement.MaxOccupants;
            housing.Rent = replacement.Rent;
            housing.UpgradeRequirements = replacement.UpgradeRequirements;
            housing.Upgrade(replacement.UpgradesTo);

            // copy over the unique id
            entity.PrototypeID = upgradedEntity.PrototypeID;
        }

        public override void Shutdown()
        {
            housingUnits.Clear();
        }
    }
}
