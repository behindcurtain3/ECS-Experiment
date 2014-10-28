using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using IsoECS.GamePlay;
using IsoECS.DataStructures;
using IsoECS.DataStructures.GamePlay;

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
            Control.ClientMargins = new Margins(4, 4, 4, 4);            

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
                Anchor = Anchors.Top | Anchors.Right
            };

            lbl = new Label(Manager)
            {
                Text = "Needs:",
                Top = numberOfEmployees.Top + numberOfEmployees.Height + 5,
                Left = numberOfEmployees.Left,
                Width = numberOfEmployees.Width,
                Parent = Control
            };

            Label inputsBox = new Label(Manager)
            {
                Text = "",
                Top = lbl.Top,
                Left = lbl.Left,
                Width = lbl.Width,
                Parent = Control,
                Alignment = Alignment.MiddleRight,
                Anchor = Anchors.Top | Anchors.Right
            };
            inputsBox.Init();

            lbl = new Label(Manager)
            {
                Text = "Produces:",
                Top = inputsBox.Top + inputsBox.Height + 5,
                Left = inputsBox.Left,
                Width = inputsBox.Width,
                Parent = Control
            };

            Label outputsBox = new Label(Manager)
            {
                Text = "",
                Top = inputsBox.Top + inputsBox.Height + 5,
                Left = inputsBox.Left,
                Width = inputsBox.Width,
                Parent = Control,
                Alignment = Alignment.MiddleRight,
                Anchor = Anchors.Top | Anchors.Right
            };
            outputsBox.Init();
            
            foreach (RecipeStage stage in recipe.Stages)
            {
                foreach (RecipeInput input in stage.Inputs)
                {
                    Item item = GameData.Instance.GetItem(input.Item);
                    string name = item.Name;

                    if (!input.Required)
                        name += " (Optional)";

                    if (!string.IsNullOrWhiteSpace(inputsBox.Text))
                        inputsBox.Text += ", ";

                    inputsBox.Text += name;
                }

                foreach (RecipeOutput output in stage.Outputs)
                {
                    string name = GameData.Instance.GetItem(output.Item).Name;

                    if (!string.IsNullOrWhiteSpace(outputsBox.Text))
                        outputsBox.Text += ", ";

                    outputsBox.Text += name;
                }
            }

            workDoneProgress = new ProgressBar(Manager)
            {
                Range = (int)recipe.Stages[Data.CurrentStage].WorkRequired,
                Value = (int)Data.WorkDone,
                Top = (int)(Control.ClientHeight * 0.75) - 10,
                Width = (int)(Control.ClientWidth * 0.75),                
                Parent = Control,
                Anchor = Anchors.Horizontal | Anchors.Bottom
            };
            workDoneProgress.Left = Control.ClientWidth / 2 - workDoneProgress.Width / 2;
            workDoneProgress.Init();

            currentStage = new Label(Manager)
            {
                Text = GetStageText(),
                Top = workDoneProgress.Top + workDoneProgress.Height + 5,
                Left = workDoneProgress.Left,
                Width = workDoneProgress.Width,
                Alignment = Alignment.MiddleCenter,
                Parent = Control,
                Anchor = Anchors.Horizontal | Anchors.Bottom
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

            Recipe recipe = GameData.Instance.GetRecipe(Data.Recipe);

            if(Data.CurrentStage < recipe.Stages.Count)
                workDoneProgress.Range = (int)recipe.Stages[Data.CurrentStage].WorkRequired;
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
