using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class HousingRenderer : DataRenderer<HousingComponent, Panel>
    {
        private Label numberOfTennants;
        private Label rent;

        public HousingRenderer(HousingComponent component, Manager manager)
            : base(component, manager)
        {
            Control = new Panel(Manager)
            {
                Passive = true,
                Color = Color.Transparent
            };
            Control.Init();

            Data.TennantAdded += new HousingComponent.HousingEventHandler(Data_TennantsChanged);
            Data.TennantRemoved += new HousingComponent.HousingEventHandler(Data_TennantsChanged);
            Data.ProspectAdded += new HousingComponent.HousingEventHandler(Data_TennantsChanged);
            Data.ProspectRemoved += new HousingComponent.HousingEventHandler(Data_TennantsChanged);
            Data.RentChanged += new HousingComponent.HousingEventHandler(Data_RentChanged);
        }

        public override Panel GetControl(Control parent)
        {
            Control.Parent = parent;
            Control.Left = 0;
            Control.Top = 0;
            Control.Width = parent.ClientWidth;
            Control.Height = parent.ClientHeight;
            Control.Anchor = Anchors.All;
            Control.ClientMargins = new Margins(4, 4, 4, 4);

            Label lbl = new Label(Manager)
            {
                Text = "Tennants:",
                Left = Control.ClientMargins.Left,
                Top = Control.ClientMargins.Top,
                Width = Control.ClientWidth - Control.ClientMargins.Horizontal,
                Parent = Control
            };

            numberOfTennants = new Label(Manager)
            {
                Text = GetTennantsText(),
                Left = lbl.Left,
                Top = lbl.Top,
                Width = lbl.Width,
                Alignment = Alignment.MiddleRight,
                Parent = Control,
                Anchor = Anchors.Top | Anchors.Right
            };

            lbl = new Label(Manager)
            {
                Text = "Rent:",
                Left = numberOfTennants.Left,
                Top = numberOfTennants.Top + numberOfTennants.Height + 5,
                Width = numberOfTennants.Width,
                Parent = Control
            };

            rent = new Label(Manager)
            {
                Text = GetRentText(),
                Left = lbl.Left,
                Top = lbl.Top,
                Width = lbl.Width,
                Parent = Control,
                Alignment = Alignment.MiddleRight,
                Anchor = Anchors.Top | Anchors.Right
            };

            return base.GetControl(parent);
        }

        private void Data_TennantsChanged(HousingComponent sender)
        {
            numberOfTennants.Text = GetTennantsText();
        }

        private void Data_RentChanged(HousingComponent sender)
        {
            rent.Text = GetRentText();
        }

        private string GetTennantsText()
        {
            return string.Format("{0}/{1}", Data.Tennants.Length, Data.MaxOccupants);
        }

        private string GetRentText()
        {
            return Data.Rent.ToString("c0");
        }
    }
}
