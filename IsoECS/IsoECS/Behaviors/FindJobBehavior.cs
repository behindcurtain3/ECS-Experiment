using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;
using Microsoft.Xna.Framework;

namespace IsoECS.Behaviors
{
    public class FindJobBehavior : Behavior
    {
        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            CitizenComponent citizen = self.Get<CitizenComponent>();
            List<Entity> potentialJobs = em.GetBuildingsWithinWalkableDistance(citizen.HousingID, 20);
            
            foreach (Entity e in potentialJobs)
            {
                ProductionComponent production = e.Get<ProductionComponent>();

                // make sure there are jobs available here
                if (production.NumEmployees >= production.MaxEmployees)
                    continue;

                // see if the job hires the appropriate gender
                if (production.EmployeeGender != Gender.BOTH && production.EmployeeGender != citizen.Gender)
                    continue;

                // take the job
                // TODO: make sure the employee can path to the job
                citizen.JobID = e.ID;
                if (production.MaxHaulers > 0)
                {
                    citizen.IsHauler = production.NumHaulers < production.MaxHaulers;
                    if(citizen.IsHauler)
                        production.Haulers.Add(self.ID);
                }
                else
                    citizen.IsHauler = false;

                production.Employees.Add(self.ID);
                Status = BehaviorStatus.SUCCESS;
                return;
            }
            
            Status = BehaviorStatus.FAILURE;
        }
    }
}
