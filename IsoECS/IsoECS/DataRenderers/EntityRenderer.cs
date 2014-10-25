using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using TomShane.Neoforce.Controls;
using IsoECS.Components.GamePlay;

namespace IsoECS.DataRenderers
{
    public class EntityRenderer : DataRenderer<Entity>
    {
        private Control displayedControl;

        public EntityRenderer(Entity e, Manager manager)
            : base(e, manager)
        {
            // Setup the window
            Control = new Dialog(Manager)
            {
                Text = "",
                Height = 300,
                Width = 450,
                Left = manager.GraphicsDevice.Viewport.Width - 450,
                MinimumWidth = 350,
                MinimumHeight = 200
            };
            Control.Init();
            ((Dialog)Control).Closed += new WindowClosedEventHandler(Window_Closed);
            ((Dialog)Control).TopPanel.Visible = false;
            ((Dialog)Control).BottomPanel.Visible = false;
        }

        public override Control GetControl()
        {
            if (displayedControl != null)
            {
                Control.Remove(displayedControl);
            }

            Control.Text = "";
            Manager.Add(Control);
            Control.Show();

            if (Data.HasComponent<BuildableComponent>())
            {
                BuildableComponent buildable = Data.Get<BuildableComponent>();
                Control.Text = buildable.Name;
            }

            if (Data.HasComponent<StockpileComponent>())
            {
                StockpileRenderer spr = new StockpileRenderer(Data.Get<StockpileComponent>(), Manager);
                displayedControl = spr.GetControl();
                displayedControl.SetPosition(0, 0);
                displayedControl.SetSize(Control.ClientWidth, Control.ClientHeight);
                displayedControl.Parent = Control;
            }

            Control.Text = string.Format("({0}) {1}", Data.ID, Control.Text);

            return base.GetControl();
        }

        public override void Shutdown()
        {
            Control.Manager.Remove(Control);
        }

        private void Window_Closed(object sender, WindowClosedEventArgs e)
        {
            Shutdown();
        }
    }
}
