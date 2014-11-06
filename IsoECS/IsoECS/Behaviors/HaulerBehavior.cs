using System;
using System.Collections.Generic;
using System.Linq;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.GamePlay;
using Microsoft.Xna.Framework;
using TecsDotNet;

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

        public override BehaviorStatus Update(Entity self, double dt)
        {
            BehaviorStatus status = base.Update(self, dt);
            CitizenComponent citizen;

            switch (status)
            {
                case BehaviorStatus.SUCCESS:
                case BehaviorStatus.FAIL:
                    if (Finished is ExitBuildingBehavior)
                    {
                        ExitBuildingBehavior exit = (ExitBuildingBehavior)Finished;

                        if (status == BehaviorStatus.SUCCESS)
                        {
                            // make sure the citizen starts at the right position
                            PositionComponent position = self.Get<PositionComponent>();
                            Vector2 startAt = World.Map.GetPositionFromIndex(exit.SelectedPath.Start.X, exit.SelectedPath.Start.Y);
                            position.X = startAt.X;
                            position.Y = startAt.Y;
                            position.Index = exit.SelectedPath.Start;

                            GoToBehavior g2b = new GoToBehavior()
                            {
                                GeneratedPath = exit.SelectedPath,
                                TargetID = exit.TargetID
                            };
                            AddChild(g2b);
                        }
                    }
                    else if (Finished is GoToBehavior)
                    {
                        if (status == BehaviorStatus.SUCCESS)
                        {
                            if (State == HaulerState.RETURNING)
                            {
                                // Add the items in out inventory to the stockpile
                                Inventory selfInventory = self.Get<Inventory>();

                                // move item back to job inventory
                                if (!string.IsNullOrWhiteSpace(CurrentItem) && selfInventory.Items[CurrentItem].Amount > 0)
                                {
                                    citizen = self.Get<CitizenComponent>();
                                    Inventory jobInventory = World.Entities.Get(citizen.JobID).Get<Inventory>();

                                    jobInventory.Add(CurrentItem, selfInventory.Items[CurrentItem].Amount);
                                    selfInventory.Set(CurrentItem, 0);
                                }

                                State = HaulerState.DELIVERING;
                                AddChild(new IdleBehavior() { IdleTime = 0.2 });
                            }
                            else if (State == HaulerState.DELIVERING)
                            {
                                // Add the items in out inventory to the stockpile
                                Inventory selfInventory = self.Get<Inventory>();
                                Entity se = World.Entities.Get(((GoToBehavior)Finished).TargetID);
                                StockpileComponent stockpile = se.Get<StockpileComponent>();

                                int movedAmount = stockpile.AddToItem(CurrentItem, selfInventory.Items[CurrentItem].Amount);
                                selfInventory.Add(CurrentItem, -movedAmount);

                                State = HaulerState.RETURNING;
                                AddChild(new IdleBehavior() { IdleTime = 0.2 });
                            }
                        }
                    }
                    break;

                case BehaviorStatus.RUN:
                    citizen = self.Get<CitizenComponent>();

                    if (State == HaulerState.RETURNING)
                    {
                        AddChild(new ExitBuildingBehavior() { ExitID = citizen.InsideID, TargetID = citizen.JobID });
                    }
                    else if (State == HaulerState.DELIVERING)
                    {
                        Inventory jobInventory = World.Entities.Get(citizen.JobID).Get<Inventory>();
                        Inventory haulerInventory = self.Get<Inventory>();

                        if (jobInventory.Items.Values.ToList().Find(delegate(InventoryData d) { return (d.Output && d.Amount > 0); }) != null)
                        {
                            List<Entity> stockpiles = World.GetBuildingsWithinWalkableDistance<StockpileComponent>(citizen.JobID, 30);
                
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
                                        AddChild(new ExitBuildingBehavior() { ExitID = citizen.InsideID, TargetID = se.ID });
                                        return BehaviorStatus.WAIT;
                                    }
                                }
                            }
                        }
                
                        // if no delivery sleep for a bit
                        AddChild(new IdleBehavior());
                    }
                    break;
            }

            return BehaviorStatus.WAIT;
        }

        public override void Init(GameWorld world, Entity self)
        {
            base.Init(world, self);

            State = HaulerState.RETURNING;
            HaulerCapacity = 50;
        }
    }
}
