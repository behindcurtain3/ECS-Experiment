using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using IsoECS.GamePlay;
using IsoECS.DataStructures;

namespace IsoECS.DataRenderers
{
    public class ProductionRenderer : DataRenderer<ProductionComponent, Panel>
    {
        private Label numberOfEmployees;
        private Label currentStage;
        private ProgressBar workDoneProgress;

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
            Data.WorkDoneChanged += new ProductionComponent.ProductionEventHandler(Data_WorkDoneChanged);
            Data.StageChanged += new ProductionComponent.ProductionEventHandler(Data_StageChanged);
        }

        public override Panel GetControl(Control parent)
        {
            Recipe recipe = GameData.Instance.GetRecipe(Data.Recipe);

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
                Text = GetEmployeeText(),
                Left = lbl.Left,
                Top = lbl.Top,
                Width = lbl.Width,
                Alignment = Alignment.MiddleRight,
                Parent = Control,
                Anchor = Anchors.Right
            };

            lbl = new Label(Manager)
            {
                Text = "Stage:",
                Left = numberOfEmployees.Left,
                Top = numberOfEmployees.Top + numberOfEmployees.Height + 5,
                Width = Control.ClientWidth - Control.ClientMargins.Horizontal,
                Parent = Control
            };

            currentStage = new Label(Manager)
            {
                Text = GetStageText(),
                Left = lbl.Left,
                Top = lbl.Top,
                Width = lbl.Width,
                Alignment = Alignment.MiddleRight,
                Parent = Control,
                Anchor = Anchors.Right
            };

            workDoneProgress = new ProgressBar(Manager)
            {
                Range = (int)recipe.Stages[Data.CurrentStage].WorkRequired,
                Value = (int)Data.WorkDone,
                Top = lbl.Top + lbl.Height + 5,
                Left = Control.ClientMargins.Left,
                Width = lbl.Width,
                Parent = Control
            };

            return Control;
        }

        public override void Shutdown()
        {
            Data.EmployeeAdded -= Data_EmployeesChanged;
            Data.EmployeeRemoved -= Data_EmployeesChanged;
            Data.WorkDoneChanged -= Data_WorkDoneChanged;
            Data.StageChanged -= Data_StageChanged;

            base.Shutdown();
        }

        private void Data_EmployeesChanged(ProductionComponent sender, Entities.Entity employee)
        {
            numberOfEmployees.Text = GetEmployeeText();
        }

        private void Data_WorkDoneChanged(ProductionComponent sender)
        {
            workDoneProgress.Value = (int)Data.WorkDone;
        }

        private void Data_StageChanged(ProductionComponent sender)
        {
            currentStage.Text = GetStageText();
        }

        private string GetEmployeeText()
        {
            return string.Format("{0}/{1} ({2:p0})", Data.Employees.Length, Data.MaxEmployees, ((double)Data.Employees.Length / (double)Data.MaxEmployees));
        }

        private string GetStageText()
        {
            Recipe recipe = GameData.Instance.GetRecipe(Data.Recipe);

            return string.Format("{0}/{1}", Data.CurrentStage + 1, recipe.Stages.Count);
        }
    }
}
