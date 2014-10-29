using System;
using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Entities;
using IsoECS.Systems.GamePlay;
using IsoECS.Systems.UI;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Systems
{
    public class ControlSystem : ISystem
    {
        // binds a key to activate/deactivate a system
        public Dictionary<Keys, Type> KeyBindings { get; private set; }

        public Dictionary<Type, ISystem> ControlSystems { get; private set; }
        public ISystem ExclusiveSystem { get; private set; }

        public ControlSystem()
        {
            KeyBindings = new Dictionary<Keys, Type>();
            KeyBindings.Add(Keys.B, typeof(ConstructionSystem));

            ControlSystems = new Dictionary<Type, ISystem>();
            ControlSystems.Add(typeof(CameraSystem), new CameraSystem());
            ControlSystems.Add(typeof(InspectionSystem), new InspectionSystem());
        }

        public void Init()
        {
            foreach (ISystem system in ControlSystems.Values)
                system.Init();
        }

        public void Shutdown()
        {
            foreach (ISystem system in ControlSystems.Values)
                system.Shutdown();
        }

        public void Update(int dt)
        {
            Entity input = EntityManager.Instance.Entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });
            KeyboardState keyboard = input.Get<InputController>().CurrentKeyboard;
            KeyboardState prevKeyboard = input.Get<InputController>().PrevKeyboard;

            // check keybindings
            foreach (KeyValuePair<Keys, Type> binding in KeyBindings)
            {
                // if the key is pressed
                if (keyboard.IsKeyDown(binding.Key) && !prevKeyboard.IsKeyDown(binding.Key))
                {
                    if (ExclusiveSystem != null)
                    {
                        // turn it off
                        ExclusiveSystem.Shutdown();
                    }

                    if (ExclusiveSystem == null || ExclusiveSystem.GetType() != binding.Value)
                    {
                        ISystem instance = (ISystem)Activator.CreateInstance(binding.Value);
                        instance.Init();
                        ExclusiveSystem = instance;
                    }
                    else
                    {
                        ExclusiveSystem = null;
                    }
                }
            }

            // Update the subsystems
            foreach (ISystem system in ControlSystems.Values)
                system.Update(dt);

            if (ExclusiveSystem != null)
                ExclusiveSystem.Update(dt);
        }
    }
}
