using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using IsoECS.Entities;
using IsoECS.Components;
using IsoECS.Systems.Renderers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IsoECS.Systems.GamePlay
{
    public class OverlaySystem : ISystem
    {
        // binds a key to activate/deactivate a system
        public Keys KeyBinding { get; private set; }
        public bool ToggledOn { get; set; }
        public GraphicsDevice Graphics { get; set; }

        public void Update(int dt)
        {
            Entity input = EntityManager.Instance.Entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });
            KeyboardState keyboard = input.Get<InputController>().CurrentKeyboard;
            KeyboardState prevKeyboard = input.Get<InputController>().PrevKeyboard;

            if (keyboard.IsKeyDown(KeyBinding) && !prevKeyboard.IsKeyDown(KeyBinding))
            {
                ToggledOn = !ToggledOn;

                SystemManager.Instance.Renderer.Shutdown();

                if (ToggledOn)
                    SystemManager.Instance.Renderer = new RenderSystem() { Graphics = Graphics, ClearColor = Color.Black };
                else
                    SystemManager.Instance.Renderer = new FoundationOverlaySystem() { Graphics = Graphics, ClearColor = Color.Black };

                SystemManager.Instance.Renderer.Init();
            }
        }

        public void Init()
        {
            KeyBinding = Keys.Q;
            ToggledOn = true;
        }

        public void Shutdown()
        {
            
        }
    }
}
