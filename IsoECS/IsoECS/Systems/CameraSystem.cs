using IsoECS.Components;
using IsoECS.Input;
using Microsoft.Xna.Framework.Input;
using TecsDotNet;

namespace IsoECS.Systems
{
    public class CameraSystem : GameSystem
    {
        public int Speed { get; set; }
        public PositionComponent Camera { get; set; }

        private double factor;

        public override void Init()
        {
            base.Init();

            Speed = 500;

            Entity camera = World.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
            Camera = camera.Get<PositionComponent>();

            World.Input.CameraUpListener.Event += new InputController.KeyboardEventHandler(Instance_CameraUp);
            World.Input.CameraDownListener.Event += new InputController.KeyboardEventHandler(Instance_CameraDown);
            World.Input.CameraLeftListener.Event += new InputController.KeyboardEventHandler(Instance_CameraLeft);
            World.Input.CameraRightListener.Event += new InputController.KeyboardEventHandler(Instance_CameraRight);
        }

        public override void Shutdown()
        {
            World.Input.CameraUpListener.Event -= Instance_CameraUp;
            World.Input.CameraDownListener.Event -= Instance_CameraDown;
            World.Input.CameraLeftListener.Event -= Instance_CameraLeft;
            World.Input.CameraRightListener.Event -= Instance_CameraRight;
        }

        public override void Update(double dt)
        {
            factor = Speed * dt;
        }

        private void Instance_CameraRight(Keys key, InputEventArgs e)
        {
            Camera.X += (float)factor;   
        }

        private void Instance_CameraLeft(Keys key, InputEventArgs e)
        {
            Camera.X -= (float)factor;
        }

        private void Instance_CameraDown(Keys key, InputEventArgs e)
        {
            Camera.Y += (float)factor;
        }

        private void Instance_CameraUp(Keys key, InputEventArgs e)
        {
            Camera.Y -= (float)factor;
        }
    }
}
