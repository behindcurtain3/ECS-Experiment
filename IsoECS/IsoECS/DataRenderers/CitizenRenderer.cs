using IsoECS.Components.GamePlay;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using IsoECS.Entities;

namespace IsoECS.DataRenderers
{
    public class CitizenRenderer : DataRenderer<CitizenComponent, Panel>
    {
        private Label age;
        private Label money;
        private Label job;
        private Label housing;
        private Label inside;

        public CitizenRenderer(CitizenComponent data, Manager manager)
            : base(data, manager)
        {
            Control = new Panel(Manager)
            {
                Passive = true,
                Color = Color.Transparent
            };
            Control.Init();

            Data.HousingChanged += new CitizenComponent.CitizenEventHandler(Data_HousingChanged);
            Data.JobChanged += new CitizenComponent.CitizenEventHandler(Data_JobChanged);
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
                Text = "Age:",
                Left = Control.ClientMargins.Left,
                Top = Control.ClientMargins.Top,
                Width = Control.ClientWidth - Control.ClientMargins.Horizontal,
                Parent = Control
            };

            age = new Label(Manager)
            {
                Text = Data.Age.ToString(),
                Left = lbl.Left,
                Top = lbl.Top,
                Width = lbl.Width,
                Parent = Control,
                Alignment = Alignment.MiddleRight,
                Anchor = Anchors.Right | Anchors.Top
            };

            lbl = new Label(Manager)
            {
                Text = "Money:",
                Left = age.Left,
                Top = age.Top + age.Height + 5,
                Width = age.Width,
                Parent = Control
            };

            money = new Label(Manager)
            {
                Text = Data.Money.ToString("c0"),
                Left = lbl.Left,
                Top = lbl.Top,
                Width = lbl.Width,
                Parent = Control,
                Alignment = Alignment.MiddleRight,
                Anchor = Anchors.Right | Anchors.Top
            };

            lbl = new Label(Manager)
            {
                Text = "Lives in:",
                Left = money.Left,
                Top = money.Top + money.Height + 5,
                Width = money.Width,
                Parent = Control
            };

            housing = new Label(Manager)
            {
                Text = GetBuildableText(Data.HousingID),
                Left = lbl.Left,
                Top = lbl.Top,
                Width = lbl.Width,
                Parent = Control,
                Alignment = Alignment.MiddleRight,
                Anchor = Anchors.Right | Anchors.Top
            };

            lbl = new Label(Manager)
            {
                Text = "Works At:",
                Left = housing.Left,
                Top = housing.Top + housing.Height + 5,
                Width = housing.Width,
                Parent = Control
            };

            job = new Label(Manager)
            {
                Text = GetBuildableText(Data.JobID),
                Left = lbl.Left,
                Top = lbl.Top,
                Width = lbl.Width,
                Parent = Control,
                Alignment = Alignment.MiddleRight,
                Anchor = Anchors.Right | Anchors.Top
            };

            return base.GetControl(parent);
        }

        private string GetBuildableText(int id)
        {
            if (id == -1)
                return "None";

            Entity e = EntityManager.Instance.GetEntity(id);

            if (e != null)
            {
                BuildableComponent house = e.Get<BuildableComponent>();

                return house.Name;
            }
            else
            {
                return "None";
            }
        }
        
        private void Data_HousingChanged(CitizenComponent sender)
        {
            housing.Text = GetBuildableText(Data.HousingID);
        }

        private void Data_JobChanged(CitizenComponent sender)
        {
            job.Text = GetBuildableText(Data.JobID);
        }
    }
}
