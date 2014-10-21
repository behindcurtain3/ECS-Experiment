using System;
using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;

namespace IsoECS.Behaviors
{
    public class FindHomeBehavior : Behavior
    {
        public int HousingID { get; set; }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            List<Entity> houses = em.Entities.FindAll(delegate(Entity e) { return e.HasComponent<HousingComponent>(); });
            CitizenComponent citizen = self.Get<CitizenComponent>();

            foreach (Entity housingEntity in houses)
            {
                HousingComponent house = housingEntity.Get<HousingComponent>();

                if (house.NumOccupantsAndProspectives >= house.MaxOccupants)
                    continue;

                // TODO: check for appropriate level of housing (rich, middle class, poor)
                if (house.Rent <= citizen.Money)
                {
                    HousingID = housingEntity.ID;
                    house.ProspectiveTennants.Add(self.ID);

                    // TODO: enter sub behavior to move into the home (find a path there and then move there)
                    state.Push(new GoToBehavior() { TargetID = HousingID });
                    return;
                }
            }

            // if no home was found enter failure state
            Status = BehaviorStatus.FAILURE;
        }

        public override void OnSubFinished(EntityManager em, Entity self, Behavior finished, Stack<Behavior> state)
        {
            base.OnSubFinished(em, self, finished, state);

            if (finished is GoToBehavior)
            {
                Entity house = em.Entities.Find(delegate(Entity e) { return e.ID == HousingID; });

                // take ourselves out of the tennant list
                house.Get<HousingComponent>().ProspectiveTennants.Remove(self.ID);

                if (finished.Status == BehaviorStatus.FAILURE)
                {
                    Status = BehaviorStatus.FAILURE;
                    
                    // reset the tracking house ID
                    HousingID = -1;
                }
                else
                {
                    house.Get<HousingComponent>().Tennants.Add(self.ID);
                    self.Get<CitizenComponent>().HousingID = HousingID;
                    state.Push(new FadeBehavior() { FadeIn = false });
                }
            }
            else
            {
                Status = finished.Status;
            }
        }
    }
}
