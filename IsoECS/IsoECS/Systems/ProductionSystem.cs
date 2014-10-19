using System;
using System.Collections.Generic;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;
using IsoECS.GamePlay;
using IsoECS.DataStructures;
using IsoECS.DataStructures.GamePlay;

namespace IsoECS.Systems
{
    public class ProductionSystem : ISystem
    {
        public void Init(EntityManager em)
        {
        }

        public void Shutdown(EntityManager em)
        {
        }

        public void Update(EntityManager em, int dt)
        {
            List<Entity> producers = em.Entities.FindAll(delegate(Entity e) { return e.HasComponent<ProductionComponent>(); });

            foreach (Entity e in producers)
            {
                ProductionComponent p = e.Get<ProductionComponent>();

                if (p.LastTick == em.Date.Time)
                    continue;

                if (string.IsNullOrWhiteSpace(p.Recipe))
                    continue;

                Recipe r = GameData.Instance.GetRecipe(p.Recipe);

                // determines how much work is done
                float workerPercentage = (float)p.Employees.Count / (float)p.MaxEmployees;
                long elapsed = em.Date.MinutesElapsed(p.LastTick);

                p.WorkDone += (elapsed * workerPercentage);
                p.LastTick = em.Date.Time;

                if (p.WorkDone >= r.Stages[p.CurrentStage].WorkRequired)
                {
                    // TODO: check the inputs and modify or elminate the output based on the amount of inputs present
                    // store output in the inventory
                    Inventory inventory = e.Get<Inventory>();
                    RecipeStage stage = r.Stages[p.CurrentStage];

                    foreach (RecipeOutput output in stage.Outputs)
                    {
                        inventory.Add(output.Item, output.AmountProduced);
                        Console.WriteLine(string.Format("#{0} has produced: {1} -> {2}", e.ID, output.Item, inventory.Items[output.Item]));
                    }

                    p.CurrentStage++;

                    if (p.CurrentStage >= r.Stages.Count)
                        p.CurrentStage = 0;

                    p.WorkDone = 0;
                }
            }
        }
    }
}
