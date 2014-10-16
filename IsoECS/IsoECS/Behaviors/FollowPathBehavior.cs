using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using IsoECS.DataStructures;
using IsoECS.Components.GamePlay;
using IsoECS.Components;
using Microsoft.Xna.Framework;
using IsoECS.Util;

namespace IsoECS.Behaviors
{
    public class FollowPathBehavior : Behavior
    {
        public Path PathToFollow { get; set; }
        public float Speed { get; set; }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
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
                Vector2 targetPos = Isometric.GetIsometricPosition(em.Map, 0, PathToFollow.Waypoints[0].Y, PathToFollow.Waypoints[0].X);
                Vector2 arrivedAt = Isometric.MoveTowards(position.Position, Speed, targetPos);
                position.X = arrivedAt.X;
                position.Y = arrivedAt.Y;

                if (position.Position == targetPos)
                {
                    PathToFollow.Waypoints.RemoveAt(0);

                    // check the next waypoint to ensure it is still valid
                    if (PathToFollow.Waypoints.Count > 0)
                    {
                        Point nextWaypoint = PathToFollow.Waypoints[0];

                        // if the next waypoint is blocked, clear the path
                        if (em.Collisions.Map[nextWaypoint] == -1)
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
