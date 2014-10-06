using System;
using System.Collections.Generic;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;

namespace IsoECS.Systems
{
    public class ProductionSystem : ISystem
    {
        public void Update(List<Entity> entities, int dt)
        {
            List<Entity> producers = entities.FindAll(delegate(Entity e) { return e.HasComponent<Generator>(); });

            foreach (Entity e in producers)
            {
                Generator g = e.Get<Generator>();

                // TODO: check to make sure the generator has the required inputs in inventory before
                // continuing the countdown
                g.RateCountdown -= dt;

                // if the countdown is less than zero a new unit has been produced
                if (g.RateCountdown < 1)
                {
                    // reset the countdown
                    g.RateCountdown += g.Rate;

                    // Add the outputs to the inventory
                    Inventory inv = e.Get<Inventory>();

                    if (inv.Items.ContainsKey(g.Recipe.Output.Name))
                    {
                        inv.Items[g.Recipe.Output.Name] += g.Recipe.Output.Amount;
                    }
                    else
                    {
                        inv.Items.Add(g.Recipe.Output.Name, g.Recipe.Output.Amount);
                    }
                }
            }
        }
    }
}
