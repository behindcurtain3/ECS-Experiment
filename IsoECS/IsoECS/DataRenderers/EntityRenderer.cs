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

        private int previousSelectedTab = 0;

        private CitizenRenderer cr;
        private HousingRenderer hr;
        private InventoryRenderer ir;
        private ProductionRenderer pr;
        private StockpileRenderer sr;

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
                Anchor = Anchors.Right,
                CanFocus = false
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
                Anchor = Anchors.Left,
                CanFocus = false
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
                Anchor = Anchors.Left,
                CanFocus = false
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
            tabs.PageChanged += new EventHandler(Tabs_PageChanged);
        }

        public override Dialog GetControl(Control parent)
        {
            Control.Text = "";
            Manager.Add(Control);
            Control.Show();

            // stop listening while redrawing
            tabs.PageChanged -= Tabs_PageChanged;

            foreach (TabPage tab in tabs.TabPages)
                tabs.RemovePage(tab);

            if (Data.HasComponent<BuildableComponent>())
            {
                BuildableComponent buildable = Data.Get<BuildableComponent>();
                Control.Text = buildable.Name;
                Control.Caption.Text = "Description";
                Control.Description.Text = buildable.Description;
            }

            if (Data.HasComponent<CitizenComponent>())
            {
                CitizenComponent citizen = Data.Get<CitizenComponent>();
                Control.Text = citizen.DisplayName;
                Control.Caption.Text = "Pleb";
                Control.Description.Text = "A lower class citizen of the city.";

                cr = new CitizenRenderer(Data.Get<CitizenComponent>(), Manager);
                TabPage tab = tabs.AddPage("Info");

                Panel cp = cr.GetControl(tab);
            }

            // Get housing control
            if (Data.HasComponent<HousingComponent>())
            {
                hr = new HousingRenderer(Data.Get<HousingComponent>(), Manager);
                TabPage tab = tabs.AddPage("Housing");

                Panel gp = hr.GetControl(tab);
            }

            // Get production control
            if (Data.HasComponent<ProductionComponent>())
            {
                pr = new ProductionRenderer(Data.Get<ProductionComponent>(), Manager);
                TabPage tab = tabs.AddPage("Production");
                Panel p = pr.GetControl(tab);
            }

            // Get stockpile
            if (Data.HasComponent<StockpileComponent>())
            {
                sr = new StockpileRenderer(Data.Get<StockpileComponent>(), Manager);
                TabPage tab = tabs.AddPage("Stockpile");

                Table t = sr.GetControl(tab);
                t.SetPosition(-tabs.ClientMargins.Left, -2);
                t.SetSize(tab.ClientWidth + tabs.ClientMargins.Horizontal, tab.ClientHeight + 2 + tabs.ClientMargins.Bottom);
                t.Parent = tab;
            }
            
            // Get inventory
            if (Data.HasComponent<Inventory>())
            {
                ir = new InventoryRenderer(Data.Get<Inventory>(), Manager);
                TabPage tab = tabs.AddPage("Inventory");

                Table it = ir.GetControl(tab);
                it.SetPosition(-tabs.ClientMargins.Left, -2);
                it.SetSize(tab.ClientWidth + tabs.ClientMargins.Horizontal, tab.ClientHeight + 2 + tabs.ClientMargins.Bottom);
                it.Parent = tab;
            }

            Control.Text = string.Format("({0}) {1}", Data.ID, Control.Text);

            if (previousSelectedTab < tabs.TabPages.Length)
                tabs.SelectedIndex = previousSelectedTab;

            tabs.PageChanged += new EventHandler(Tabs_PageChanged);

            return Control;
        }

        public override void Shutdown()
        {
            if (cr != null)
                cr.Shutdown();

            if (hr != null)
                hr.Shutdown();

            if (ir != null)
                ir.Shutdown();

            if (pr != null)
                pr.Shutdown();

            if (sr != null)
                sr.Shutdown();

            Control.Manager.Remove(Control);
        }

        public Window Update(Entity data)
        {
            Data = data;
            return GetControl(null);
        }

        private void Window_Closed(object sender, WindowClosedEventArgs e)
        {
            Shutdown();
        }

        private void Tabs_PageChanged(object sender, EventArgs e)
        {
            previousSelectedTab = tabs.SelectedIndex;
        }

        private void btnOkay_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Shutdown();
        }
    }
}
