using System;
using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Entities;
using IsoECS.Systems.GamePlay;
using IsoECS.Systems.UI;
using Microsoft.Xna.Framework.Input;
using IsoECS.Systems.Threaded;
using IsoECS.Input;

namespace IsoECS.Systems
{
    public class ControlSystem : ISystem
    {
        public ISystem ExclusiveSystem { get; private set; }

        public ControlSystem()
        {
        }

        public void Init()
        {
            InputController.Instance.ConstructionMode.Event += new InputController.KeyboardEventHandler(Instance_ConstructionMode);
        }

        public void Shutdown()
        {
            InputController.Instance.ConstructionMode.Event -= Instance_ConstructionMode;
        }

        private void Instance_ConstructionMode(Keys key, InputEventArgs e)
        {
            if (ExclusiveSystem != null)
                ExclusiveSystem.Shutdown();

            if (ExclusiveSystem is ConstructionSystem)
            {
                ExclusiveSystem = null;
            }
            else
            {
                ExclusiveSystem = new ConstructionSystem();
                ExclusiveSystem.Init();
            }
        }

        public void Update(int dt)
        {
            if (ExclusiveSystem != null)
                ExclusiveSystem.Update(dt);

            /*
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
             */
        }
    }
}
