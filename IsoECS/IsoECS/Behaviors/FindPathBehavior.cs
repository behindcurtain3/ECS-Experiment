using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.Systems.Threaded;
using Microsoft.Xna.Framework;

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

        public override BehaviorStatus Update(Entity self, int dt)
        {
            BehaviorStatus status = base.Update(self, dt);

            switch (status)
            {
                case BehaviorStatus.SUCCESS:
                case BehaviorStatus.FAIL:
                    break;

                case BehaviorStatus.RUN:
                    Entity target = EntityManager.Instance.Entities.Find(delegate(Entity e) { return e.ID == TargetID; });

                    // if invalid target fail out
                    if (target == null)
                    {
                        return BehaviorStatus.FAIL;
                    }

                    if (PathRequest != null)
                    {
                        // try to retrieve the path
                        Path p = PathfinderSystem.GetPath(PathRequest.ID);

                        if (p == null)
                        {
                            // idle for a short bit
                            AddChild(new IdleBehavior() { IdleTime = 100 });
                            return BehaviorStatus.WAIT;
                        }
                        else
                        {
                            GeneratedPath = p;

                            // TODO: check for tPosition = sPosition
                            if (GeneratedPath.Waypoints.Count == 0)
                            {
                                return BehaviorStatus.FAIL;
                            }

                            return BehaviorStatus.SUCCESS;
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
                                return BehaviorStatus.FAIL;
                            }

                            PositionComponent tPosition = target.Get<PositionComponent>();
                            PositionComponent sPosition = self.Get<PositionComponent>();

                            if (tPosition.Index == null || tPosition.Index == Point.Zero)
                                tPosition.Index = EntityManager.Instance.Map.GetIndexFromPosition((int)tPosition.X, (int)tPosition.Y);

                            PathRequest = new PathRequest()
                            {
                                Start = sPosition.Index,
                                End = tPosition.Index,
                                Validation = AnyPathNotBlocked
                            };
                            PathfinderSystem.RequestPath(PathRequest);
                        }
                        else
                        {
                            // check for a starting position and ensure the target has a foundation
                            if (!self.HasComponent<PositionComponent>() || !target.HasComponent<FoundationComponent>() || !target.HasComponent<PositionComponent>())
                            {
                                return BehaviorStatus.FAIL;
                            }

                            PositionComponent tPosition = target.Get<PositionComponent>();
                            FoundationComponent foundation = target.Get<FoundationComponent>();

                            // list to hold valid road tiles to move to
                            List<Point> validLandings = EntityManager.Instance.GetValidExitsFromFoundation(target);

                            if (validLandings.Count == 0)
                            {
                                return BehaviorStatus.FAIL;
                            }
                            else
                            {
                                // find a multi-path
                                PositionComponent sPosition = self.Get<PositionComponent>();
                                PathRequest = new PathRequest()
                                {
                                    Start = sPosition.Index,
                                    Ends = validLandings,
                                    Validation = OnlyRoads
                                };
                                PathfinderSystem.RequestPath(PathRequest);
                            }
                        }
                    }       
                    break;
            }

            return BehaviorStatus.WAIT;
        }

        private bool AnyPathNotBlocked(Point current)
        {
            return EntityManager.Instance.Collisions.Map[current] != PathTypes.BLOCKED;
        }

        private bool OnlyRoads(Point current)
        {
            return EntityManager.Instance.Collisions.Map[current] == PathTypes.ROAD;
        }
    }
}
