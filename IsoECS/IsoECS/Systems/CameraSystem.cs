using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Entities;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Systems
{
    public class CameraSystem : ISystem
    {
        public void Init()
        {
        }

        public void Shutdown()
        {
        }
        public void Update(int dt)
        {
            Entity camera = EntityManager.Instance.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
            Entity input = EntityManager.Instance.Entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });

            if (camera != null)
            {
                CameraController controller = camera.Get<CameraController>();
                KeyboardState keyboard = input.Get<InputController>().CurrentKeyboard;
                PositionComponent position = camera.Get<PositionComponent>();
                int speed = 10;

                bool up, down, left, right;
                up = down = left = right = false;

                // check each type of key
                foreach (Keys key in controller.Up)
                {
                    if (keyboard.IsKeyDown(key))
                    {
                        up = true;
                        break;
                    }
                }

                foreach (Keys key in controller.Down)
                {
                    if (keyboard.IsKeyDown(key))
                    {
                        down = true;
                        break;
                    }
                }

                foreach (Keys key in controller.Left)
                {
                    if (keyboard.IsKeyDown(key))
                    {
                        left = true;
                        break;
                    }
                }

                foreach (Keys key in controller.Right)
                {
                    if (keyboard.IsKeyDown(key))
                    {
                        right = true;
                        break;
                    }
                }

                if (up) position.Y -= speed;
                if (down) position.Y += speed;
                if (left) position.X -= speed;
                if (right) position.X += speed;
            }
        }
    }
}
