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

            // get all homes with vacanies
            List<Entity> vacantHomes = new List<Entity>();

            foreach (Entity housingEntity in houses)
            {
                HousingComponent house = housingEntity.Get<HousingComponent>();

                if (house.Tennants.Count >= house.MaxOccupants)
                    continue;

                // TODO: check for appropriate level of housing (rich, middle class, poor)
                if (house.Rent <= citizen.Money)
                {
                    HousingID = housingEntity.ID;
                    house.Tennants.Add(self.ID);

                    // TODO: enter sub behavior to move into the home (find a path there and then move there)
                    FindPathBehavior fpb = new FindPathBehavior()
                    {
                        TargetID = housingEntity.ID,
                        MoveToNearbyRoad = true
                    };
                    state.Push(fpb);
                    Console.WriteLine("Home found for citizen: " + String.Format("#{0}-{1}", self.ID, citizen.Name));
                    return;
                }
            }

            // if no home was found enter failure state
            Status = BehaviorStatus.FAILURE;
        }

        public override void OnSubFinished(EntityManager em, Entity self, Behavior finished, Stack<Behavior> state)
        {
            base.OnSubFinished(em, self, finished, state);

            if (finished is FindPathBehavior)
            {
                FindPathBehavior fpb = (FindPathBehavior)finished;

                if (fpb.GeneratedPath.Waypoints.Count > 0)
                {
                    FollowPathBehavior follow = new FollowPathBehavior()
                    {
                        PathToFollow = fpb.GeneratedPath,
                        Speed = 1.5f
                    };
                    state.Push(follow);
                    Console.WriteLine("Path found to home for citizen: " + String.Format("#{0}", self.ID));
                }
                else
                {
                    Status = BehaviorStatus.FAILURE;
                    Console.WriteLine("NO Path found to home for citizen: " + String.Format("#{0}", self.ID));

                    Entity house = em.Entities.Find(delegate(Entity e) { return e.ID == HousingID; });

                    // take ourselves out of the tennant list
                    house.Get<HousingComponent>().Tennants.Remove(self.ID);

                    // reset the tracking house ID
                    HousingID = -1;
                }
            }
            else if (finished is FollowPathBehavior)
            {
                if (finished.Status == BehaviorStatus.SUCCESS)
                {
                    self.Get<CitizenComponent>().HousingID = HousingID;
                    Status = BehaviorStatus.SUCCESS;
                    Console.WriteLine("Finished following a path: " + String.Format("#{0}-{1}", self.ID, Status));

                    // Fadeout
                    state.Push(new FadeBehavior() { FadeIn = false });
                }
                else
                {
                    // failed to follow path to home
                    // TOOD: possibly fail out to decide to do something else?
                    Status = BehaviorStatus.RUNNING;

                    Entity house = em.Entities.Find(delegate(Entity e) { return e.ID == HousingID; });

                    // take ourselves out of the tennant list
                    house.Get<HousingComponent>().Tennants.Remove(self.ID);

                    // reset the tracking house ID
                    HousingID = -1;
                }
            }
            else
            {
                Status = finished.Status;
            }
        }
    }
}
