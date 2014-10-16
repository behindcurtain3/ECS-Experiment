using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;
using IsoECS.Util;
using Microsoft.Xna.Framework;

namespace IsoECS.Systems.GamePlay
{
    public class MoveToLocationSystem : ISystem
    {
        private CollisionMapComponent Collisions { get; set; }
        private IsometricMapComponent Map { get; set; }

        public void Init(EntityManager em)
        {
            Entity dataTracker = em.Entities.Find(delegate(Entity e) { return e.HasComponent<RoadPlannerComponent>(); });
            Collisions = dataTracker.Get<CollisionMapComponent>();

            Entity mapEntity = em.Entities.Find(delegate(Entity e) { return e.HasComponent<IsometricMapComponent>(); });
            Map = mapEntity.Get<IsometricMapComponent>();
        }

        public void Shutdown(EntityManager em)
        {
        }

        public void Update(EntityManager em, int dt)
        {
            List<Entity> movingEntities = em.Entities.FindAll(delegate(Entity e) { return e.HasComponent<MoveToTargetComponent>(); });
            
            foreach (Entity e in movingEntities)
            {
                MoveToTargetComponent moveable = e.Get<MoveToTargetComponent>();
                PositionComponent position = e.Get<PositionComponent>();

                if (moveable.PathToTarget == null || moveable.PathToTarget.Waypoints.Count <= 0)
                {
                    // stop moving
                    e.RemoveComponent(moveable);
                }
                else
                {
                    // move to the target
                    Vector2 targetPos = Isometric.GetIsometricPosition(Map, 0, moveable.PathToTarget.Waypoints[0].Y, moveable.PathToTarget.Waypoints[0].X);
                    Vector2 arrivedAt = Isometric.MoveTowards(position.Position, moveable.Speed, targetPos);
                    position.X = arrivedAt.X;
                    position.Y = arrivedAt.Y;

                    if (position.Position == targetPos)
                    {
                        moveable.PathToTarget.Waypoints.RemoveAt(0);

                        // check the next waypoint to ensure it is still valid
                        if (moveable.PathToTarget.Waypoints.Count > 0)
                        {
                            Point nextWaypoint = moveable.PathToTarget.Waypoints[0];

                            // if the next waypoint is blocked, clear the path
                            if (Collisions.Map[nextWaypoint] == -1)
                            {
                                // TODO: alert some failure state? Recalculate the path?
                                moveable.PathToTarget = null;
                            }
                        }
                    }
                }
            }
        }
    }
}
