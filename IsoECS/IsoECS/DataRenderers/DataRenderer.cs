using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class DataRenderer<T, S>
    {
        public virtual T Data { get; set; }
        public virtual S Control { get; set; }
        public Manager Manager { get; private set; }

        public DataRenderer(T data, Manager manager)
        {
            Data = data;
            Manager = manager;
        }

        public virtual S GetControl()
        {
            return Control;
        }

        public virtual void Shutdown()
        {
        }

        public S Update(T data)
        {
            Data = data;
            return GetControl();
        }
    }
}
