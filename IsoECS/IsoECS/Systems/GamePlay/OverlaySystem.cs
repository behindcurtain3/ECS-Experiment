using IsoECS.Input;
using IsoECS.Systems.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Systems.GamePlay
{
    public class OverlaySystem : ISystem
    {
        // binds a key to activate/deactivate a system
        public bool ToggledOn { get; set; }
        public GraphicsDevice Graphics { get; set; }

        public void Update(int dt)
        {
        }

        public void Init()
        {
            ToggledOn = true;

            InputController.Instance.DesireabilityOverlay.Event += new InputController.KeyboardEventHandler(Instance_DesireabilityOverlay);
        }

        public void Shutdown()
        {
            InputController.Instance.DesireabilityOverlay.Event -= Instance_DesireabilityOverlay;
        }

        private void Instance_DesireabilityOverlay(Keys key, InputEventArgs e)
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
}
