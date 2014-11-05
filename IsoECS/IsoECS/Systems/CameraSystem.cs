using IsoECS.Components;
using IsoECS.Entities;
using IsoECS.Input;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Systems
{
    public class CameraSystem : ISystem
    {
        public int Speed { get; set; }
        public PositionComponent Camera { get; set; }

        public void Init()
        {
            Speed = 10;

            Entity camera = EntityManager.Instance.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
            Camera = camera.Get<PositionComponent>();

            InputController.Instance.CameraUpListener.Event += new InputController.KeyboardEventHandler(Instance_CameraUp);
            InputController.Instance.CameraDownListener.Event += new InputController.KeyboardEventHandler(Instance_CameraDown);
            InputController.Instance.CameraLeftListener.Event += new InputController.KeyboardEventHandler(Instance_CameraLeft);
            InputController.Instance.CameraRightListener.Event += new InputController.KeyboardEventHandler(Instance_CameraRight);
        }

        public void Shutdown()
        {
            InputController.Instance.CameraUpListener.Event -= Instance_CameraUp;
            InputController.Instance.CameraDownListener.Event -= Instance_CameraDown;
            InputController.Instance.CameraLeftListener.Event -= Instance_CameraLeft;
            InputController.Instance.CameraRightListener.Event -= Instance_CameraRight;
        }

        public void Update(int dt)
        {
        }

        private void Instance_CameraRight(Keys key, InputEventArgs e)
        {
            Camera.X += Speed;   
        }

        private void Instance_CameraLeft(Keys key, InputEventArgs e)
        {
            Camera.X -= Speed;
        }

        private void Instance_CameraDown(Keys key, InputEventArgs e)
        {
            Camera.Y += Speed;
        }

        private void Instance_CameraUp(Keys key, InputEventArgs e)
        {
            Camera.Y -= Speed;
        }
    }
}
