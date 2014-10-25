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
        private Button btnOkay;
        private Control displayedControl;
        private TabControl tabs;

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

            btnOkay = new Button(Manager)
            {
                Text = "Ok",
                Left = Control.BottomPanel.ClientWidth - 85,
                Top = (Control.BottomPanel.ClientHeight / 2) - 11,
                Height = 23,
                Width = 75,
                Parent = Control.BottomPanel,
                Anchor = Anchors.Right
            };
            btnOkay.Init();
            btnOkay.Click += new TomShane.Neoforce.Controls.EventHandler(btnOkay_Click);

            tabs = new TabControl(Manager)
            {
                Left = 2,
                Top = Control.TopPanel.Height,
                Width = Control.ClientWidth - 2,
                Height = Control.ClientHeight - Control.TopPanel.Height - Control.BottomPanel.Height,
                Anchor = Anchors.All
            };
            tabs.Init();
            tabs.Parent = Control;
        }

        private void btnOkay_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Shutdown();
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
            foreach (TabPage tab in tabs.TabPages)
                tabs.RemovePage(tab);

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
                TabPage tab = tabs.AddPage("Stockpile");

                Table t = spr.GetControl();
                t.SetPosition(-tabs.ClientMargins.Left, -2);
                t.SetSize(tab.ClientWidth + tabs.ClientMargins.Horizontal, tab.ClientHeight + 2 + tabs.ClientMargins.Bottom);
                t.Parent = tab;
            }
            else if (Data.HasComponent<HousingComponent>())
            {
                HousingRenderer hr = new HousingRenderer(Data.Get<HousingComponent>(), Manager);
                TabPage tab = tabs.AddPage("Housing");

                Panel gp = hr.GetControl();
                gp.Parent = tab;
            }
            /*
            if (displayedControl != null)
            {
                displayedControl.SetPosition(2, Control.TopPanel.Height);
                displayedControl.SetSize(Control.ClientWidth - 2, Control.ClientHeight - Control.TopPanel.Height - Control.BottomPanel.Height);
                displayedControl.Parent = Control;
            }
            */

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
