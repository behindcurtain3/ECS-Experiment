﻿using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;
using Microsoft.Xna.Framework;

namespace IsoECS.Behaviors
{
    public class FindJobBehavior : Behavior
    {
        public override void Update(Entity self, Stack<Behavior> state, int dt)
        {
            CitizenComponent citizen = self.Get<CitizenComponent>();
            List<Entity> potentialJobs = EntityManager.Instance.GetBuildingsWithinWalkableDistance<ProductionComponent>(citizen.HousingID, 20);
            
            foreach (Entity e in potentialJobs)
            {
                ProductionComponent production = e.Get<ProductionComponent>();

                // make sure there are jobs available here
                if (production.Employees.Length >= production.MaxEmployees)
                    continue;

                // see if the job hires the appropriate gender
                if (production.EmployeeGender != Gender.BOTH && production.EmployeeGender != citizen.Gender)
                    continue;

                // take the job
                // TODO: make sure the employee can path to the job
                if (production.AddEmployee(self))
                {
                    citizen.JobID = e.ID;
                    if (production.MaxHaulers > 0)
                    {
                        citizen.IsHauler = production.AddHauler(self);

                        if (citizen.IsHauler)
                        {
                            self.AddComponent(new Inventory()); // add an inventory to the hauler
                        }
                    }
                    Status = BehaviorStatus.SUCCESS;
                    return;
                }
            }
            
            Status = BehaviorStatus.FAILURE;
        }
    }
}
