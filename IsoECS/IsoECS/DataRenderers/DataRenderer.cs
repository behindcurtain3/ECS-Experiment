using TomShane.Neoforce.Controls;
using IsoECS.GamePlay;

namespace IsoECS.DataRenderers
{
    public class DataRenderer<T, S>
    {
        public virtual T Data { get; set; }
        public virtual S Control { get; set; }
        public Manager Manager { get; private set; }
        public GameWorld World { get; private set; }

        public DataRenderer(T data, GameWorld world)
        {
            Data = data;
            World = world;
            Manager = world.UI;
        }

        public virtual S GetControl(Control parent)
        {
            return Control;
        }

        public virtual void Shutdown()
        {
        }
    }
}
