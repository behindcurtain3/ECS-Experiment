using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using IsoECS.Components;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Systems
{
    public class InputSystem : ISystem
    {
        public void Update(List<Entity> entities, int dt)
        {
            List<Entity> inputControls = entities.FindAll(delegate(Entity e) { return e.HasComponent<InputController>(); });

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
