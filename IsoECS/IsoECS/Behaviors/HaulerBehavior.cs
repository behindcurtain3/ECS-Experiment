﻿using System;
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

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            CitizenComponent citizen = self.Get<CitizenComponent>();

            if (State == HaulerState.RETURNING)
            {
                state.Push(new ExitBuildingBehavior() { ExitID = citizen.InsideID, TargetID = citizen.JobID });
            }
            else if (State == HaulerState.DELIVERING)
            {
                List<Entity> stockpiles = em.GetBuildingsWithinWalkableDistance<StockpileComponent>(citizen.JobID, 30);
                Inventory jobInventory = em.GetEntity(citizen.JobID).Get<Inventory>();
                Inventory haulerInventory = self.Get<Inventory>();

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
                            jobInventory.Items[invItem.Item].Amount -= amount;
                            CurrentItem = invItem.Item;
                            state.Push(new ExitBuildingBehavior() { ExitID = citizen.InsideID, TargetID = se.ID });
                            return;
                        }
                    }
                }

                // if no delivery sleep for a bit
                state.Push(new IdleBehavior() { IdleTime = 500 });
            }
        }

        public override void Init(EntityManager em, Entity self)
        {
            base.Init(em, self);

            State = HaulerState.RETURNING;
            HaulerCapacity = 50;
        }

        public override void OnSubFinished(EntityManager em, Entity self, Behavior finished, Stack<Behavior> state)
        {
            base.OnSubFinished(em, self, finished, state);

            if (finished is ExitBuildingBehavior)
            {
                ExitBuildingBehavior exit = (ExitBuildingBehavior)finished;

                if (exit.Status == BehaviorStatus.SUCCESS)
                {
                    // make sure the citizen starts at the right position
                    PositionComponent position = self.Get<PositionComponent>();
                    Vector2 startAt = Isometric.GetIsometricPosition(em.Map, 0, exit.SelectedPath.Start.Y, exit.SelectedPath.Start.X);
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
                            Inventory jobInventory = em.GetEntity(citizen.JobID).Get<Inventory>();

                            jobInventory.Add(CurrentItem, selfInventory.Items[CurrentItem].Amount);
                            selfInventory.Items[CurrentItem].Amount = 0;
                        }

                        State = HaulerState.DELIVERING;
                        state.Push(new IdleBehavior() { IdleTime = 200 });
                    }
                    else if (State == HaulerState.DELIVERING)
                    {
                        // Add the items in out inventory to the stockpile
                        Inventory selfInventory = self.Get<Inventory>();
                        Entity se = em.GetEntity(((GoToBehavior)finished).TargetID);
                        StockpileComponent stockpile = se.Get<StockpileComponent>();

                        int movedAmount = stockpile.AddToItem(CurrentItem, selfInventory.Items[CurrentItem].Amount);
                        selfInventory.Items[CurrentItem].Amount -= movedAmount;

                        State = HaulerState.RETURNING;
                        state.Push(new IdleBehavior() { IdleTime = 200 });
                    }
                }
            }
        }
    }
}