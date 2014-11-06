using IsoECS.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TecsDotNet;
using IsoECS.GamePlay;
using IsoECS.RenderSystems;

namespace IsoECS.Systems.GamePlay
{
    public class OverlaySystem : GameSystem
    {
        // binds a key to activate/deactivate a system
        public bool ToggledOn { get; set; }
        public GraphicsDevice Graphics { get; set; }

        public override void Update(double dt)
        {
        }

        public override void Init()
        {
            ToggledOn = true;

            World.Input.DesireabilityOverlay.Event += new InputController.KeyboardEventHandler(Instance_DesireabilityOverlay);
        }

        public override void Shutdown()
        {
            World.Input.DesireabilityOverlay.Event -= Instance_DesireabilityOverlay;
        }

        private void Instance_DesireabilityOverlay(Keys key, InputEventArgs e)
        {
            ToggledOn = !ToggledOn;

            if (World.Renderer != null)
                World.Systems.Remove(World.Renderer);

            if (ToggledOn)
                World.Renderer = new DefaultRenderSystem() { Graphics = Graphics, ClearColor = Color.Black };
            else
                World.Renderer = new FoundationOverlaySystem() { Graphics = Graphics, ClearColor = Color.Black };

            World.Systems.Add(World.Renderer);
        }
    }
}
