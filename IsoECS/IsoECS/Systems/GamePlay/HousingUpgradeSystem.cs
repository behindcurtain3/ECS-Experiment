using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Components;
using IsoECS.Util;

namespace IsoECS.Systems.GamePlay
{
    public class HousingUpgradeSystem : ISystem
    {
        private List<Entity> housingUnits = new List<Entity>();

        public void Update(int dt)
        {
        }

        public void Init()
        {
            housingUnits.AddRange(EntityManager.Instance.Entities.FindAll(delegate(Entity e) { return e.HasComponent<HousingComponent>(); }));

            EntityManager.Instance.EntityAdded += new EntityManager.EntityEventHandler(Instance_EntityAdded);
            EntityManager.Instance.EntityRemoved -= new EntityManager.EntityEventHandler(Instance_EntityRemoved);
        }

        private void Instance_EntityAdded(Entity e)
        {
            if (e.HasComponent<HousingComponent>())
            {
                housingUnits.Add(e);

                HousingComponent housing = e.Get<HousingComponent>();
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

            // swap out the HousingComponent with the upgraded version and the DrawableComponent
            housing.BelongsTo.RemoveComponent(housing.BelongsTo.Get<DrawableComponent>());
            housing.BelongsTo.RemoveComponent(housing.BelongsTo.Get<BuildableComponent>());

            // copy out the entity to upgrade to from the library
            Entity upgradedEntity = Serialization.DeepCopy<Entity>(EntityLibrary.Instance.Get(housing.UpgradesTo));

            // copy in the new components
            housing.BelongsTo.AddComponent(Serialization.DeepCopy<DrawableComponent>(upgradedEntity.Get<DrawableComponent>()));
            housing.BelongsTo.AddComponent(Serialization.DeepCopy<BuildableComponent>(upgradedEntity.Get<BuildableComponent>()));

            // make sure the housing tennant data is copied over
            HousingComponent replacement = Serialization.DeepCopy<HousingComponent>(upgradedEntity.Get<HousingComponent>());

            // copy over housing data
            housing.MaxOccupants = replacement.MaxOccupants;
            housing.Rent = replacement.Rent;
            housing.UpgradeRequirements = replacement.UpgradeRequirements;
            housing.Upgrade(replacement.UpgradesTo);

            // copy over the unique id
            housing.BelongsTo.UniqueID = upgradedEntity.UniqueID;
        }

        private void Instance_EntityRemoved(Entity e)
        {
            if (e.HasComponent<HousingComponent>())
                e.Get<HousingComponent>().TennantAdded -= Housing_TennantAdded;

            housingUnits.Remove(e);
        }

        public void Shutdown()
        {
            housingUnits.Clear();
        }
    }
}
