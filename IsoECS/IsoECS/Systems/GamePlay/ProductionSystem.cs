using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.DataStructures.GamePlay;
using IsoECS.GamePlay;
using TecsDotNet;

namespace IsoECS.Systems
{
    public class ProductionSystem : GameSystem
    {
        private long lastUpdate;

        public override void Init()
        {
            base.Init();
            lastUpdate = World.Date.Time;
            World.Date.TimeChanged += new GameDateComponent.GameDateEventHandler(Date_TimeChanged);
        }

        public override void Shutdown()
        {
            World.Date.TimeChanged -= Date_TimeChanged;
        }

        public override void Update(double dt)
        {   
        }

        private void Date_TimeChanged(GameDateComponent sender)
        {
            long elapsed = World.Date.MinutesElapsed(lastUpdate);
            lastUpdate = World.Date.Time;

            List<Entity> producers = World.Entities.FindAll(delegate(Entity e) { return e.HasComponent<ProductionComponent>(); });

            foreach (Entity e in producers)
            {
                ProductionComponent p = e.Get<ProductionComponent>();

                if (string.IsNullOrWhiteSpace(p.Recipe))
                    continue;

                // determines how much work is done
                // TODO: check for divide by zero?
                float workerPercentage = (float)p.Employees.Length / (float)p.MaxEmployees;

                p.WorkDone += (elapsed * workerPercentage);

                Recipe r = (Recipe)World.Prototypes[p.Recipe];
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
                        drawable.RemoveByPrototypeID(str);
                    }

                    // add
                    foreach (string str in stage.AddToDrawableComponent)
                    {
                        GameDrawable igd = (GameDrawable)World.Prototypes[str];

                        drawable.Add(igd.Layer, igd);
                    }
                }
            }
        }
    }
}
