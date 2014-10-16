using System;
using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;

namespace IsoECS.Behaviors
{
    public class FindHomeBehavior : Behavior
    {
        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            List<Entity> houses = em.Entities.FindAll(delegate(Entity e) { return e.HasComponent<HousingComponent>(); });
            CitizenComponent citizen = self.Get<CitizenComponent>();

            // get all homes with vacanies
            List<Entity> vacantHomes = new List<Entity>();

            foreach (Entity housingEntity in houses)
            {
                HousingComponent house = housingEntity.Get<HousingComponent>();

                if (house.Tennants.Count < house.MaxOccupants)
                    vacantHomes.Add(housingEntity);
            }

            if (vacantHomes.Count == 0)
            {
                // if there are no vacant homes,fail out of this behavior
                Status = BehaviorStatus.FAILURE;
                return;
            }

            // if they are attempt to find a living place
            // TODO: check for appropriate level of housing (rich, middle class, poor)
            foreach (Entity potentialHome in vacantHomes)
            {
                HousingComponent home = potentialHome.Get<HousingComponent>();

                if (home.Rent <= citizen.Money && home.Tennants.Count < home.MaxOccupants)
                {
                    citizen.HousingID = (int)potentialHome.ID;
                    home.Tennants.Add((int)self.ID);
                    Status = BehaviorStatus.SUCCESS;
                    Console.WriteLine("Home found for citizen: " + String.Format("#{0}-{1}", self.ID, citizen.Name));
                    return;
                }
            }
        }
    }
}
