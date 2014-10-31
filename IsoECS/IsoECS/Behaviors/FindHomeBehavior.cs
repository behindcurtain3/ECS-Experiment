using System;
using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;

namespace IsoECS.Behaviors
{
    public class FindHomeBehavior : Behavior
    {
        public int HousingID { get; set; }

        public override BehaviorStatus Update(Entity self, int dt)
        {
            BehaviorStatus status = base.Update(self, dt);

            switch (status)
            {
                case BehaviorStatus.SUCCESS:
                case BehaviorStatus.FAIL:
                    if (Finished is GoToBehavior)
                    {
                        Entity house = EntityManager.Instance.Entities.Find(delegate(Entity e) { return e.ID == HousingID; });

                        // take ourselves out of the tennant list
                        house.Get<HousingComponent>().RemoveProspect(self);

                        if (status == BehaviorStatus.FAIL)
                        {
                            // reset the tracking house ID
                            HousingID = -1;
                            return BehaviorStatus.FAIL;
                        }
                        else
                        {
                            house.Get<HousingComponent>().AddTennant(self);
                            self.Get<CitizenComponent>().HousingID = HousingID;
                            AddChild(new FadeBehavior() { FadeIn = false });
                        }
                    }
                    else
                    {
                        return status;
                    }
                    break;

                case BehaviorStatus.RUN:
                    List<Entity> houses = EntityManager.Instance.Entities.FindAll(delegate(Entity e) { return e.HasComponent<HousingComponent>(); });
                    CitizenComponent citizen = self.Get<CitizenComponent>();

                    foreach (Entity housingEntity in houses)
                    {
                        HousingComponent house = housingEntity.Get<HousingComponent>();

                        if (house.SpaceAvailable <= 0)
                            continue;

                        // TODO: check for appropriate level of housing (rich, middle class, poor)
                        if (house.Rent <= citizen.Money)
                        {
                            HousingID = housingEntity.ID;
                            house.AddProspect(self);

                            // TODO: enter sub behavior to move into the home (find a path there and then move there)
                            AddChild(new GoToBehavior() { TargetID = HousingID });
                            return BehaviorStatus.WAIT;
                        }
                    }

                    // if no home was found enter failure state
                    return BehaviorStatus.FAIL;
            }

            return BehaviorStatus.WAIT;
        }
    }
}
