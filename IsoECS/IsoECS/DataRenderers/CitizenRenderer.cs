using IsoECS.Components.GamePlay;
using IsoECS.GamePlay;
using Microsoft.Xna.Framework;
using TecsDotNet;
using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class CitizenRenderer : DataRenderer<CitizenComponent, Panel>
    {
        private Label age;
        private Label money;
        private Label job;
        private Label housing;

        public CitizenRenderer(CitizenComponent data, GameWorld world)
            : base(data, world)
        {
            Control = new Panel(Manager)
            {
                Passive = true,
                Color = Color.Transparent
            };
            Control.Init();

            Data.HousingChanged += new CitizenComponent.CitizenEventHandler(Data_HousingChanged);
            Data.JobChanged += new CitizenComponent.CitizenEventHandler(Data_JobChanged);
            Data.AgeChanged += new CitizenComponent.CitizenEventHandler(Data_AgeChanged);
            Data.MoneyChanged += new CitizenComponent.CitizenEventHandler(Data_MoneyChanged);
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
                Text = GetBuildableText((int)Data.HousingID),
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
                Text = GetBuildableText((int)Data.JobID),
                Left = lbl.Left,
                Top = lbl.Top,
                Width = lbl.Width,
                Parent = Control,
                Alignment = Alignment.MiddleRight,
                Anchor = Anchors.Right | Anchors.Top
            };

            return base.GetControl(parent);
        }

        public override void Shutdown()
        {
            Data.AgeChanged -= Data_AgeChanged;
            Data.MoneyChanged -= Data_MoneyChanged;
            Data.JobChanged -= Data_JobChanged;
            Data.HousingChanged -= Data_HousingChanged;

            base.Shutdown();
        }

        private string GetBuildableText(int id)
        {
            if (id == 0)
                return "None";

            Entity e = World.Entities.Get((uint)id);

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
            housing.Text = GetBuildableText((int)Data.HousingID);
        }

        private void Data_JobChanged(CitizenComponent sender)
        {
            job.Text = GetBuildableText((int)Data.JobID);
        }

        private void Data_MoneyChanged(CitizenComponent sender)
        {
            money.Text = Data.Money.ToString("c0");
        }

        private void Data_AgeChanged(CitizenComponent sender)
        {
            age.Text = Data.Age.ToString();
        }
    }
}
