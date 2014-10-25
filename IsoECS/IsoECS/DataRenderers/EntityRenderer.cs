using IsoECS.Components.GamePlay;
using IsoECS.Entities;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class EntityRenderer : DataRenderer<Entity, Dialog>
    {
        private Button btnOkay;
        private TabControl tabs;

        public Button Next { get; private set; }
        public Button Previous { get; private set; }

        public EntityRenderer(Entity e, Manager manager)
            : base(e, manager)
        {
            // Setup the window
            Control = new Dialog(Manager)
            {
                Text = "",
                Height = 350,
                Width = 550,
                MinimumWidth = 350,
                MinimumHeight = 200
            };
            Control.Init();
            Control.Center();
            Control.Caption.TextColor = Control.Description.TextColor = new Color(228, 228, 228);
            Control.Closed += new WindowClosedEventHandler(Window_Closed);

            btnOkay = new Button(Manager)
            {
                Text = "Ok",
                Left = Control.BottomPanel.ClientWidth - 58,
                Top = (Control.BottomPanel.ClientHeight / 2) - 11,
                Height = 23,
                Width = 48,
                Parent = Control.BottomPanel,
                Anchor = Anchors.Right
            };
            btnOkay.Init();
            btnOkay.Click += new TomShane.Neoforce.Controls.EventHandler(btnOkay_Click);

            Previous = new Button(Manager)
            {
                Text = "< Previous",
                Left = 10,
                Height = 23,
                Width = 96,
                Top = btnOkay.Top,
                Parent = Control.BottomPanel,
                Anchor = Anchors.Left
            };
            Previous.Init();

            Next = new Button(Manager)
            {
                Text = "Next >",
                Left = (Previous.Left * 2) + Previous.Width,
                Height = Previous.Height,
                Width = Previous.Width,
                Top = Previous.Top,
                Parent = Control.BottomPanel,
                Anchor = Anchors.Left
            };
            Next.Init();

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

            // Get housing control
            if (Data.HasComponent<HousingComponent>())
            {
                HousingRenderer hr = new HousingRenderer(Data.Get<HousingComponent>(), Manager);
                TabPage tab = tabs.AddPage("Housing");

                Panel gp = hr.GetControl();
                gp.Parent = tab;
            }

            // Get stockpile
            if (Data.HasComponent<StockpileComponent>())
            {
                StockpileRenderer spr = new StockpileRenderer(Data.Get<StockpileComponent>(), Manager);
                TabPage tab = tabs.AddPage("Stockpile");

                Table t = spr.GetControl();
                t.SetPosition(-tabs.ClientMargins.Left, -2);
                t.SetSize(tab.ClientWidth + tabs.ClientMargins.Horizontal, tab.ClientHeight + 2 + tabs.ClientMargins.Bottom);
                t.Parent = tab;
            }
            
            // Get inventory
            if (Data.HasComponent<Inventory>())
            {
                InventoryRenderer ir = new InventoryRenderer(Data.Get<Inventory>(), Manager);
                TabPage tab = tabs.AddPage("Inventory");

                Table it = ir.GetControl();
                it.SetPosition(-tabs.ClientMargins.Left, -2);
                it.SetSize(tab.ClientWidth + tabs.ClientMargins.Horizontal, tab.ClientHeight + 2 + tabs.ClientMargins.Bottom);
                it.Parent = tab;
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
