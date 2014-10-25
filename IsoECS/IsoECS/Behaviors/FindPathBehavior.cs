using System;
using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using IsoECS.Systems.Threaded;

namespace IsoECS.Behaviors
{
    public class FindPathBehavior : Behavior
    {
        // the ID of the target entity to move towards
        public int TargetID { get; set; }

        // if true just find a path to a road tile next to the target
        // if false find a path to the exact position
        public bool MoveToNearbyRoad { get; set; }

        // the path generated
        public Path GeneratedPath { get; private set; }

        public PathRequest PathRequest { get; set; }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            Entity target = em.Entities.Find(delegate(Entity e) { return e.ID == TargetID; });

            // if invalid target fail out
            if (target == null)
            {
                Status = BehaviorStatus.FAILURE;
                return;
            }

            if (PathRequest != null)
            {
                // try to retrieve the path
                Path p = PathfinderSystem.GetPath(PathRequest.ID);

                if (p == null)
                {
                    // idle for a short bit
                    state.Push(new IdleBehavior() { IdleTime = 100 });
                    return;
                }
                else
                {
                    GeneratedPath = p;

                    // TODO: check for tPosition = sPosition
                    if (GeneratedPath.Waypoints.Count == 0)
                    {
                        Status = BehaviorStatus.FAILURE;
                        return;
                    }

                    Status = BehaviorStatus.SUCCESS;
                }
            }
            else
            {
                // make sure the target has a position or foundation
                if (!MoveToNearbyRoad)
                {
                    // if the target or self has no position fail out
                    if (!target.HasComponent<PositionComponent>() || !self.HasComponent<PositionComponent>())
                    {
                        Status = BehaviorStatus.FAILURE;
                        return;
                    }

                    PositionComponent tPosition = target.Get<PositionComponent>();
                    PositionComponent sPosition = self.Get<PositionComponent>();

                    if (tPosition.Index == null || tPosition.Index == Point.Zero)
                        tPosition.Index = em.Map.GetIndexFromPosition((int)tPosition.X, (int)tPosition.Y);

                    PathRequest = new PathRequest()
                    {
                        Start = sPosition.Index,
                        End = tPosition.Index
                    };
                    PathfinderSystem.RequestPath(PathRequest);
                }
                else
                {
                    // check for a starting position and ensure the target has a foundation
                    if (!self.HasComponent<PositionComponent>() || !target.HasComponent<FoundationComponent>() || !target.HasComponent<PositionComponent>())
                    {
                        Status = BehaviorStatus.FAILURE;
                        return;
                    }

                    PositionComponent tPosition = target.Get<PositionComponent>();
                    FoundationComponent foundation = target.Get<FoundationComponent>();

                    // list to hold valid road tiles to move to
                    List<Point> validLandings = em.GetValidExitsFromFoundation(target);

                    if (validLandings.Count == 0)
                    {
                        Status = BehaviorStatus.FAILURE;
                        return;
                    }
                    else
                    {
                        // find a multi-path
                        PositionComponent sPosition = self.Get<PositionComponent>();
                        PathRequest = new PathRequest()
                        {
                            Start = sPosition.Index,
                            Ends = validLandings
                        };
                        PathfinderSystem.RequestPath(PathRequest);
                    }
                }
            }                
        }
    }
}
