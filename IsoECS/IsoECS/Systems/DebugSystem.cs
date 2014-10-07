using System.Collections.Generic;
using IsoECS.Entities;
using IsoECS.Components;
using Microsoft.Xna.Framework;
using IsoECS.Util;
using IsoECS.Components.GamePlay;
using System;

namespace IsoECS.Systems
{
    public class DebugSystem : ISystem
    {
        public void Update(List<Entity> entities, int dt)
        {
            Entity debugger = entities.Find(delegate(Entity e) { return e.HasComponent<DebugComponent>(); });
            Entity inputEntity = entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });
            Entity cameraEntity = entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
            Entity mapEntity = entities.Find(delegate(Entity e) { return e.HasComponent<IsometricMapComponent>(); });

            if (debugger != null)
            {
                InputController input = inputEntity.Get<InputController>();
                PositionComponent camera = cameraEntity.Get<PositionComponent>();
                DebugComponent debug = debugger.Get<DebugComponent>();
                PositionComponent debugPosition = debugger.Get<PositionComponent>();
                IsometricMapComponent map = mapEntity.Get<IsometricMapComponent>();

                // set the starting coords
                int x = input.CurrentMouse.X + (int)camera.X;
                int y = input.CurrentMouse.Y + (int)camera.Y;

                // pick out the tile index that the screen coords intersect
                Point index = Isometric.GetPointAtScreenCoords(map, x, y);

                // if the index is not valid hide it
                if (index.X < 0 || index.X >= map.TxWidth || index.Y < 0 || index.Y >= map.TxHeight)
                    debugger.Get<DrawableComponent>().Visible = false;
                else
                    debugger.Get<DrawableComponent>().Visible = true;

                // translate the index into a screen position
                Vector2 dPositiion = Isometric.GetIsometricPosition(map, 0, index.Y, index.X);

                // update the debug sprite position accounting for the camera
                debugPosition.X = dPositiion.X - (int)camera.X;
                debugPosition.Y = dPositiion.Y - (int)camera.Y;

                //debugger.Get<DrawableTextComponent>().Text = string.Format("{0}x{1}\n{2}x{3}\n{4}x{5}\n{6}x{7}", x, y, index.X, index.Y, camera.X, camera.Y, dPositiion.X, dPositiion.Y);
            }
        }
    }
}
