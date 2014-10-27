using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class ProductionRenderer : DataRenderer<ProductionComponent, Panel>
    {
        private Label numberOfEmployees;

        public ProductionRenderer(ProductionComponent data, Manager manager)
            : base(data, manager)
        {
            Control = new Panel(Manager)
            {
                Passive = true,
                Color = Color.Transparent
            };
            Control.Init();

            Data.EmployeeAdded += new ProductionComponent.EmployeeEventHandler(Data_EmployeesChanged);
            Data.EmployeeRemoved += new ProductionComponent.EmployeeEventHandler(Data_EmployeesChanged);
        }

        public override Panel GetControl(Control parent)
        {
            Control.Parent = parent;
            Control.Left = 0;
            Control.Top = 0;
            Control.Width = parent.ClientWidth;
            Control.Height = parent.ClientHeight;
            Control.Anchor = Anchors.All;

            Label lbl = new Label(Manager)
            {
                Text = "Number of Employees:",
                Left = Control.ClientMargins.Left,
                Top = Control.ClientMargins.Top,
                Width = Control.ClientWidth - Control.ClientMargins.Horizontal,
                Parent = Control
            };

            numberOfEmployees = new Label(Manager)
            {
                Text = Data.Employees.Length.ToString(),
                Left = lbl.Left,
                Top = lbl.Top,
                Width = lbl.Width,
                Alignment = Alignment.MiddleRight,
                Parent = Control,
                Anchor = Anchors.Right
            };

            return Control;
        }

        public override void Shutdown()
        {
            Data.EmployeeAdded -= Data_EmployeesChanged;
            Data.EmployeeRemoved -= Data_EmployeesChanged;

            base.Shutdown();
        }

        private void Data_EmployeesChanged(ProductionComponent sender, Entities.Entity employee)
        {
            numberOfEmployees.Text = sender.Employees.Length.ToString();
        }
    }
}
