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
        public void Update(EntityManager em, int dt)
        {
            List<Entity> entities = em.Entities.FindAll(delegate(Entity e) { return e.HasComponent<HousingComponent>(); });

            foreach (Entity e in entities)
            {
                HousingComponent housing = e.Get<HousingComponent>();

                if (string.IsNullOrWhiteSpace(housing.UpgradesTo) || housing.UpgradeRequirements == null)
                    continue;

                // check each requirement to see if it has been set
                // then check if the requirement fails, if it does just continue to the next entity
                if (housing.UpgradeRequirements.MinimumOccupants != -1)
                {
                    if (housing.Tennants.Length < housing.UpgradeRequirements.MinimumOccupants)
                    {
                        continue;
                    }
                }

                // swap out the HousingComponent with the upgraded version and the DrawableComponent
                e.RemoveComponent(e.Get<DrawableComponent>());
                e.RemoveComponent(e.Get<BuildableComponent>());

                // copy out the entity to upgrade to from the library
                Entity upgradedEntity = Serialization.DeepCopy<Entity>(EntityLibrary.Instance.Get(housing.UpgradesTo));

                // copy in the new components
                e.AddComponent(Serialization.DeepCopy<DrawableComponent>(upgradedEntity.Get<DrawableComponent>()));                
                e.AddComponent(Serialization.DeepCopy<BuildableComponent>(upgradedEntity.Get<BuildableComponent>()));

                // make sure the housing tennant data is copied over
                HousingComponent replacement = Serialization.DeepCopy<HousingComponent>(upgradedEntity.Get<HousingComponent>());

                // copy over housing data
                housing.MaxOccupants = replacement.MaxOccupants;
                housing.Rent = replacement.Rent;
                housing.UpgradesTo = replacement.UpgradesTo;
                housing.UpgradeRequirements = replacement.UpgradeRequirements;

                // copy over the unique id
                e.UniqueID = upgradedEntity.UniqueID;
            }
        }

        public void Init(EntityManager em)
        {
        }

        public void Shutdown(EntityManager em)
        {
        }
    }
}
