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

                if (house.Tennants.Count >= house.MaxOccupants)
                    continue;

                // TODO: check for appropriate level of housing (rich, middle class, poor)
                if (house.Rent <= citizen.Money)
                {
                    citizen.HousingID = (int)housingEntity.ID;
                    house.Tennants.Add((int)self.ID);

                    // TODO: enter sub behavior to move into the home (find a path there and then move there)
                    FindPathBehavior fpb = new FindPathBehavior();
                    fpb.TargetID = housingEntity.ID;
                    fpb.MoveToNearbyRoad = true;
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
                    FollowPathBehavior follow = new FollowPathBehavior();
                    follow.PathToFollow = fpb.GeneratedPath;
                    follow.Speed = 1.5f;
                    state.Push(follow);
                    Console.WriteLine("Path found to home for citizen: " + String.Format("#{0}", self.ID));
                }
                else
                {
                    Status = BehaviorStatus.FAILURE;
                    Console.WriteLine("NO Path found to home for citizen: " + String.Format("#{0}", self.ID));
                }
            }
            else if (finished is FollowPathBehavior)
            {
                Status = finished.Status;
                Console.WriteLine("Finished finding a path: " + String.Format("#{0}-{1}", self.ID, Status));
            }
        }
    }
}
