using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;

namespace IsoECS.Behaviors
{
    public class FindJobBehavior : Behavior
    {
        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            List<Entity> potentialJobs = em.Entities.FindAll(delegate(Entity e) { return e.HasComponent<ProductionComponent>(); });
            CitizenComponent citizen = self.Get<CitizenComponent>();

            foreach (Entity e in potentialJobs)
            {
                ProductionComponent production = e.Get<ProductionComponent>();

                // make sure there are jobs available here
                if (production.Employees.Count >= production.MaxEmployees)
                    continue;

                // see if the job hires the appropriate gender
                if (production.EmployeeGender != Gender.BOTH && production.EmployeeGender != citizen.Gender)
                    continue;

                // take the job
                // TODO: make sure the employee can path to the job
                citizen.JobID = e.ID;
                production.Employees.Add(self.ID);
                Status = BehaviorStatus.SUCCESS;
                return;
            }

            Status = BehaviorStatus.FAILURE;
        }
    }
}
