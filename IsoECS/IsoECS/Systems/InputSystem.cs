using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Entities;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Systems
{
    public class InputSystem : ISystem
    {
        public void Init()
        {
        }

        public void Shutdown()
        {
        }

        public void Update(int dt)
        {
            List<Entity> inputControls = EntityManager.Instance.Entities.FindAll(delegate(Entity e) { return e.HasComponent<InputController>(); });

            foreach (Entity e in inputControls)
            {
                InputController controller = e.Get<InputController>();

                controller.PrevKeyboard = controller.CurrentKeyboard;
                controller.CurrentKeyboard = Keyboard.GetState();

                controller.PrevMouse = controller.CurrentMouse;
                controller.CurrentMouse = Mouse.GetState();
            }
        }
    }
}
