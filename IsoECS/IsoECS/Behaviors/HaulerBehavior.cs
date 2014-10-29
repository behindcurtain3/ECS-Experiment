using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;
using IsoECS.Components;
using Microsoft.Xna.Framework;
using IsoECS.Util;

namespace IsoECS.Behaviors
{
    public enum HaulerState
    {
        RETURNING,
        DELIVERING
    }

    public class HaulerBehavior : Behavior
    {
        public HaulerState State { get; set; }
        public int HaulerCapacity { get; set; }
        public string CurrentItem { get; set; }

        public override void Update(Entity self, Stack<Behavior> state, int dt)
        {
            CitizenComponent citizen = self.Get<CitizenComponent>();

            if (State == HaulerState.RETURNING)
            {
                state.Push(new ExitBuildingBehavior() { ExitID = citizen.InsideID, TargetID = citizen.JobID });
            }
            else if (State == HaulerState.DELIVERING)
            {
                Inventory jobInventory = EntityManager.Instance.GetEntity(citizen.JobID).Get<Inventory>();
                Inventory haulerInventory = self.Get<Inventory>();

                if (jobInventory.Items.Values.ToList().Find(delegate(InventoryData d) { return (d.Output && d.Amount > 0); }) != null)
                {
                    List<Entity> stockpiles = EntityManager.Instance.GetBuildingsWithinWalkableDistance<StockpileComponent>(citizen.JobID, 30);
                
                    foreach (Entity se in stockpiles)
                    {
                        StockpileComponent stockpile = se.Get<StockpileComponent>();

                        foreach (InventoryData invItem in jobInventory.Items.Values)
                        {
                            if(!invItem.Output)
                                continue;

                            // do the delivery
                            if (stockpile.IsAccepting(invItem.Item) && stockpile.Amount(invItem.Item) < stockpile.Maximum(invItem.Item) && invItem.Amount > 0)
                            {
                                int amount = Math.Min(invItem.Amount, HaulerCapacity);
                                haulerInventory.Add(invItem.Item, amount);
                                jobInventory.Add(invItem.Item, -amount);
                                CurrentItem = invItem.Item;
                                state.Push(new ExitBuildingBehavior() { ExitID = citizen.InsideID, TargetID = se.ID });
                                return;
                            }
                        }
                    }
                }
                
                // if no delivery sleep for a bit
                state.Push(new IdleBehavior() { IdleTime = (int)(EntityManager.Random.NextDouble() * 5000) });
            }
        }

        public override void Init(Entity self)
        {
            base.Init(self);

            State = HaulerState.RETURNING;
            HaulerCapacity = 50;
        }

        public override void OnSubFinished(Entity self, Behavior finished, Stack<Behavior> state)
        {
            base.OnSubFinished(self, finished, state);

            if (finished is ExitBuildingBehavior)
            {
                ExitBuildingBehavior exit = (ExitBuildingBehavior)finished;

                if (exit.Status == BehaviorStatus.SUCCESS)
                {
                    // make sure the citizen starts at the right position
                    PositionComponent position = self.Get<PositionComponent>();
                    Vector2 startAt = EntityManager.Instance.Map.GetPositionFromIndex(exit.SelectedPath.Start.X, exit.SelectedPath.Start.Y);
                    position.X = startAt.X;
                    position.Y = startAt.Y;
                    position.Index = exit.SelectedPath.Start;

                    GoToBehavior g2b = new GoToBehavior()
                    {
                        GeneratedPath = exit.SelectedPath,
                        TargetID = exit.TargetID
                    };
                    state.Push(g2b);
                }
            }
            else if (finished is GoToBehavior)
            {
                if (finished.Status == BehaviorStatus.SUCCESS)
                {
                    if (State == HaulerState.RETURNING)
                    {
                        // Add the items in out inventory to the stockpile
                        Inventory selfInventory = self.Get<Inventory>();

                        // move item back to job inventory
                        if (!string.IsNullOrWhiteSpace(CurrentItem) && selfInventory.Items[CurrentItem].Amount > 0)
                        {
                            CitizenComponent citizen = self.Get<CitizenComponent>();
                            Inventory jobInventory = EntityManager.Instance.GetEntity(citizen.JobID).Get<Inventory>();

                            jobInventory.Add(CurrentItem, selfInventory.Items[CurrentItem].Amount);
                            selfInventory.Set(CurrentItem, 0);
                        }

                        State = HaulerState.DELIVERING;
                        state.Push(new IdleBehavior() { IdleTime = 200 });
                    }
                    else if (State == HaulerState.DELIVERING)
                    {
                        // Add the items in out inventory to the stockpile
                        Inventory selfInventory = self.Get<Inventory>();
                        Entity se = EntityManager.Instance.GetEntity(((GoToBehavior)finished).TargetID);
                        StockpileComponent stockpile = se.Get<StockpileComponent>();

                        int movedAmount = stockpile.AddToItem(CurrentItem, selfInventory.Items[CurrentItem].Amount);
                        selfInventory.Add(CurrentItem, -movedAmount);

                        State = HaulerState.RETURNING;
                        state.Push(new IdleBehavior() { IdleTime = 200 });
                    }
                }
            }
        }
    }
}
