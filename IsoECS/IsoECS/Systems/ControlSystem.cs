using System;
using System.Collections.Generic;
using IsoECS.Entities;
using Microsoft.Xna.Framework.Input;
using IsoECS.Components;

namespace IsoECS.Systems
{
    public class ControlSystem : ISystem
    {
        // binds a key to activate/deactivate a system
        public Dictionary<Keys, Type> KeyBindings { get; private set; }

        public Dictionary<Type, ISystem> ControlSystems { get; private set; }

        public ControlSystem()
        {
            KeyBindings = new Dictionary<Keys, Type>();
            KeyBindings.Add(Keys.C, typeof(CameraSystem));

            ControlSystems = new Dictionary<Type, ISystem>();
            ControlSystems.Add(typeof(CameraSystem), new CameraSystem());
        }

        public void Update(List<Entity> entities, int dt)
        {

            // Update the subsystems
            foreach (ISystem system in ControlSystems.Values)
                system.Update(entities, dt);

            Entity input = entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });
            KeyboardState keyboard = input.Get<InputController>().CurrentKeyboard;
            KeyboardState prevKeyboard = input.Get<InputController>().PrevKeyboard;

            foreach (KeyValuePair<Keys, Type> binding in KeyBindings)
            {
                // if the key is pressed
                if (keyboard.IsKeyDown(binding.Key) && !prevKeyboard.IsKeyDown(binding.Key))
                {
                    if (ControlSystems.ContainsKey(binding.Value))
                        ControlSystems.Remove(binding.Value);
                    else
                    {
                        ISystem instance = (ISystem)Activator.CreateInstance(binding.Value);
                        ControlSystems.Add(instance.GetType(), instance);
                    }
                }
            }
        }
    }
}
