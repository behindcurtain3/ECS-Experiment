using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;
using IsoECS.Util;
using Microsoft.Xna.Framework;

namespace IsoECS.Systems
{
    public class FollowMouseSystem : ISystem
    {
        public void Update(List<Entity> entities, int dt)
        {
            Entity inputEntity = entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });
            Entity cameraEntity = entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
            Entity mapEntity = entities.Find(delegate(Entity e) { return e.HasComponent<IsometricMapComponent>(); });
            Entity dataTracker = entities.Find(delegate(Entity e) { return e.HasComponent<RoadplannerComponent>(); });
            CollisionMapComponent collisionMap = dataTracker.Get<CollisionMapComponent>();
            InputController input = inputEntity.Get<InputController>();
            PositionComponent camera = cameraEntity.Get<PositionComponent>();
            IsometricMapComponent map = mapEntity.Get<IsometricMapComponent>();
            
            // set the starting coords
            int x = input.CurrentMouse.X + (int)camera.X;
            int y = input.CurrentMouse.Y + (int)camera.Y;

            // pick out the tile index that the screen coords intersect
            Point mouseIndex = Isometric.GetPointAtScreenCoords(map, x, y);

            List<Entity> followers = entities.FindAll(delegate(Entity e) { return e.HasComponent<MoveToTargetComponent>(); });

            foreach (Entity e in followers)
            {
                MoveToTargetComponent moveable = e.Get<MoveToTargetComponent>();
                PositionComponent position = e.Get<PositionComponent>();

                if (moveable.PathToTarget == null || moveable.PathToTarget.Waypoints.Count <= 0)
                {
                    // if not a valid index exit
                    if (!Isometric.ValidIndex(map, mouseIndex.X, mouseIndex.Y))
                        continue;

                    Point eIndex = Isometric.GetPointAtScreenCoords(map, (int)position.X, (int)position.Y);
                    
                    // already has the path to target
                    if (mouseIndex == eIndex)
                        continue;

                    if (moveable.PathToTarget != null && moveable.PathToTarget.End == mouseIndex)
                        continue;

                    moveable.PathToTarget = Pathfinder.Generate(collisionMap, map, eIndex, mouseIndex);
                    moveable.Target = mouseIndex;
                }
                else
                {
                    // move to the target
                    Vector2 targetPos = Isometric.GetIsometricPosition(map, 0, moveable.PathToTarget.Waypoints[0].Y, moveable.PathToTarget.Waypoints[0].X);
                    Vector2 arrivedAt = Isometric.MoveTowards(position.Position, 1f, targetPos);
                    position.X = arrivedAt.X;
                    position.Y = arrivedAt.Y;

                    if (position.Position == targetPos)
                    {
                        moveable.PathToTarget.Waypoints.RemoveAt(0);
                    }
                }
            }
        }
    }
}
