using System;
using System.Collections.Generic;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;
using IsoECS.GamePlay;

namespace IsoECS.Systems
{
    public class ProductionSystem : ISystem
    {
        public void Init(List<Entity> entities)
        {

        }

        public void Shutdown(List<Entity> entities)
        {

        }

        public void Update(List<Entity> entities, int dt)
        {
            List<Entity> producers = entities.FindAll(delegate(Entity e) { return e.HasComponent<Generator>(); });

            foreach (Entity e in producers)
            {
                Generator g = e.Get<Generator>();

                // check the inputs to see if this producer should run
                bool doGeneration = true;

                // if this generator runs on percentage it always generates
                // else check the inventories
                if(!g.GeneratesPercentage)
                {
                    Inventory inv = e.Get<Inventory>();
                    /*
                    // check each input item
                    foreach (Item item in g.Recipe.Input)
                    {
                        // if it doesn't exist don't proceed
                        if (!inv.Items.ContainsKey(item.Name))
                        {
                            doGeneration = false;
                            break;
                        }

                        // if there isn't enough don't proceed
                        if (inv.Items[item.Name] < item.Amount)
                        {
                            doGeneration = false;
                            break;
                        }

                    }*/
                }

                if (doGeneration)
                {
                    // continuing the countdown
                    g.RateCountdown -= dt;

                    // if the countdown is less than zero a new unit has been produced
                    if (g.RateCountdown < 1)
                    {
                        // reset the countdown
                        g.RateCountdown += g.Rate;

                        // Add the outputs to the inventory
                        Inventory inv = e.Get<Inventory>();
                        /*
                        if (inv.Items.ContainsKey(g.Recipe.Output.Name))
                        {
                            inv.Items[g.Recipe.Output.Name] += g.Recipe.Output.Amount;
                        }
                        else
                        {
                            inv.Items.Add(g.Recipe.Output.Name, g.Recipe.Output.Amount);
                        }*/
                    }
                }
            }
        }
    }
}
