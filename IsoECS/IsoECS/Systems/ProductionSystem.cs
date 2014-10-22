using System;
using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.DataStructures.GamePlay;
using IsoECS.Entities;
using IsoECS.GamePlay;
using IsoECS.Components;

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

                // determines how much work is done
                // TODO: check for divide by zero?
                float workerPercentage = (float)p.Employees.Count / (float)p.MaxEmployees;
                long elapsed = em.Date.MinutesElapsed(p.LastTick);

                p.WorkDone += (elapsed * workerPercentage);
                p.LastTick = em.Date.Time;

                Recipe r = GameData.Instance.GetRecipe(p.Recipe);
                if (p.WorkDone >= r.Stages[p.CurrentStage].WorkRequired)
                {
                    // TODO: check the inputs and modify or elminate the output based on the amount of inputs present
                    // store output in the inventory
                    Inventory inventory = e.Get<Inventory>();
                    RecipeStage stage = r.Stages[p.CurrentStage];

                    foreach (RecipeOutput output in stage.Outputs)
                    {
                        inventory.Add(output.Item, output.AmountProduced);
                        // make sure the newly produced item is marked as output
                        inventory.Items[output.Item].Output = true;
                    }

                    // go to next stage in the recipe
                    p.CurrentStage++;
                    if (p.CurrentStage >= r.Stages.Count)
                        p.CurrentStage = 0;

                    // reset the work done
                    p.WorkDone = 0;

                    // update the drawables
                    DrawableComponent drawable = e.Get<DrawableComponent>();

                    // remove
                    foreach (string str in stage.RemoveFromDrawableComponent)
                    {
                        drawable.RemoveByUniqueID(str);
                    }
                    
                    // add
                    foreach (string str in stage.AddToDrawableComponent)
                    {
                        IGameDrawable igd = DrawableLibrary.Instance.Get(str);

                        drawable.Add(igd.Layer, igd);
                    }
                }
            }
        }
    }
}
