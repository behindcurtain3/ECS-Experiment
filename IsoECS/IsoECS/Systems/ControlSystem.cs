using IsoECS.Input;
using IsoECS.Systems.GamePlay;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Systems
{
    public class ControlSystem : GameSystem
    {
        public GameSystem ExclusiveSystem { get; private set; }

        public ControlSystem()
        {
        }

        public override void Init()
        {
            base.Init();

            World.Input.ConstructionMode.Event += new InputController.KeyboardEventHandler(Instance_ConstructionMode);
        }

        public override void Shutdown()
        {
            World.Input.ConstructionMode.Event -= Instance_ConstructionMode;
        }

        private void Instance_ConstructionMode(Keys key, InputEventArgs e)
        {
            if (ExclusiveSystem is ConstructionSystem)
            {
                World.Systems.Remove(ExclusiveSystem);
                ExclusiveSystem = null;
            }
            else
            {
                ExclusiveSystem = new ConstructionSystem();
                World.Systems.Add(ExclusiveSystem);
            }
        }

        public override void Update(double dt)
        {
        }
    }
}
