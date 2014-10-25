using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using TomShane.Neoforce.Controls;
using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework;

namespace IsoECS.DataRenderers
{
    public class EntityRenderer : DataRenderer<Entity, Dialog>
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
            Control.Caption.TextColor = Control.Description.TextColor = new Color(228, 228, 228);
            Control.Closed += new WindowClosedEventHandler(Window_Closed);
        }

        public override Dialog GetControl()
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
                Control.Caption.Text = "Description";
                Control.Description.Text = buildable.Description;
            }

            if (Data.HasComponent<StockpileComponent>())
            {
                StockpileRenderer spr = new StockpileRenderer(Data.Get<StockpileComponent>(), Manager);
                displayedControl = spr.GetControl();
                displayedControl.SetPosition(2, Control.TopPanel.Height);
                displayedControl.SetSize(Control.ClientWidth - 2, Control.ClientHeight - Control.TopPanel.Height - Control.BottomPanel.Height);
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
