﻿using IsoECS.Components;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;
using TecsDotNet;

namespace IsoECS.Behaviors
{
    public class FollowPathBehavior : Behavior
    {
        public Path PathToFollow { get; set; }
        public float Speed { get; set; }

        public override BehaviorStatus Update(Entity self, double dt)
        {
            BehaviorStatus status = base.Update(self, dt);

            switch (status)
            {
                case BehaviorStatus.RUN:
                    PositionComponent position = self.Get<PositionComponent>();

                    if (PathToFollow == null)
                    {
                        return BehaviorStatus.FAIL;
                    }
                    else
                    {
                        // move to the target
                        Vector2 targetPos = World.Map.GetPositionFromIndex(PathToFollow.Waypoints[0].X, PathToFollow.Waypoints[0].Y);
                        Vector2 arrivedAt = World.Map.MoveTowards(position.Position, Speed, targetPos);
                        position.X = arrivedAt.X;
                        position.Y = arrivedAt.Y;
                
                        if (position.Position == targetPos)
                        {
                            position.Index = PathToFollow.Waypoints[0];
                            PathToFollow.Waypoints.RemoveAt(0);

                            // check the next waypoint to ensure it is still valid
                            if (PathToFollow.Waypoints.Count > 0)
                            {
                                Point nextWaypoint = PathToFollow.Waypoints[0];

                                // if the next waypoint is blocked, clear the path
                                if (World.Collisions.Map[nextWaypoint] == PathTypes.BLOCKED)
                                {
                                    // TODO: alert some failure state? Recalculate the path?
                                    return BehaviorStatus.FAIL;
                                }
                            }
                            else
                            {
                                return BehaviorStatus.SUCCESS;
                            }
                        }
                    }
                    break;
            }

            return BehaviorStatus.WAIT;
        }
    }
}
