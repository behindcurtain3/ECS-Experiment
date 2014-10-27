using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class HousingRenderer : DataRenderer<HousingComponent, Panel>
    {
        public HousingRenderer(HousingComponent component, Manager manager)
            : base(component, manager)
        {
            Control = new Panel(Manager)
            {
                Passive = true,
                Color = Color.Transparent,
                Anchor = Anchors.All
            };
            Control.Init();
        }

        public override Panel GetControl(Control parent)
        {
            
            return base.GetControl(parent);
        }
    }
}
