using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class DataRenderer<T>
    {
        public T Data { get; set; }
        public Control Control { get; set; }

        public DataRenderer(T data)
        {
            Data = data;
        }

        public virtual void Show(Manager manager)
        {
        }

        public virtual void Shutdown(Manager manager)
        {
            // remvoe the control from the scene
            if(Control != null)
                manager.Remove(Control);
        }
    }
}
