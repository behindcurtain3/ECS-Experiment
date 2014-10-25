using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class DataRenderer<T>
    {
        public virtual T Data { get; set; }
        public virtual Control Control { get; set; }
        public Manager Manager { get; private set; }

        public DataRenderer(T data, Manager manager)
        {
            Data = data;
            Manager = manager;
        }

        public virtual Control GetControl()
        {
            return Control;
        }

        public virtual void Shutdown()
        {
            // remvoe the control from the scene
            Manager.Remove(Control);
        }

        public Control Update(T data)
        {
            Data = data;
            return GetControl();
        }
    }
}
