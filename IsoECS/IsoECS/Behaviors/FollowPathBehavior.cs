using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.Util;
using Microsoft.Xna.Framework;

namespace IsoECS.Behaviors
{
    public class FollowPathBehavior : Behavior
    {
        public Path PathToFollow { get; set; }
        public float Speed { get; set; }

        public override void Update(Entity self, Stack<Behavior> state, int dt)
        {
            PositionComponent position = self.Get<PositionComponent>();

            if (PathToFollow == null)
            {
                Status = BehaviorStatus.FAILURE;
                return;
            }
            else
            {
                // move to the target
                Vector2 targetPos = EntityManager.Instance.Map.GetPositionFromIndex(PathToFollow.Waypoints[0].X, PathToFollow.Waypoints[0].Y);
                Vector2 arrivedAt = EntityManager.Instance.Map.MoveTowards(position.Position, Speed, targetPos);
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
                        if (EntityManager.Instance.Collisions.Map[nextWaypoint] == PathTypes.BLOCKED)
                        {
                            // TODO: alert some failure state? Recalculate the path?
                            Status = BehaviorStatus.FAILURE;
                            return;
                        }
                    }
                    else
                    {
                        Status = BehaviorStatus.SUCCESS;
                        return;
                    }
                }
            }
        }
    }
}
