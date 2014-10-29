using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.DataStructures.GamePlay;
using IsoECS.Entities;
using IsoECS.GamePlay;

namespace IsoECS.Systems
{
    public class ProductionSystem : ISystem
    {
        private long lastUpdate;

        public void Init()
        {
            lastUpdate = EntityManager.Instance.Date.Time;
            EntityManager.Instance.Date.TimeChanged += new GameDateComponent.GameDateEventHandler(Date_TimeChanged);
        }

        public void Shutdown()
        {
        }

        public void Update(int dt)
        {   
        }

        private void Date_TimeChanged(GameDateComponent sender)
        {
            long elapsed = EntityManager.Instance.Date.MinutesElapsed(lastUpdate);
            lastUpdate = EntityManager.Instance.Date.Time;

            List<Entity> producers = EntityManager.Instance.Entities.FindAll(delegate(Entity e) { return e.HasComponent<ProductionComponent>(); });

            foreach (Entity e in producers)
            {
                ProductionComponent p = e.Get<ProductionComponent>();

                if (string.IsNullOrWhiteSpace(p.Recipe))
                    continue;

                // determines how much work is done
                // TODO: check for divide by zero?
                float workerPercentage = (float)p.Employees.Length / (float)p.MaxEmployees;

                p.WorkDone += (elapsed * workerPercentage);

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
                    stage = r.Stages[p.CurrentStage];

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
