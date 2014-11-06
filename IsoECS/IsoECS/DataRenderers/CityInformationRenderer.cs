using IsoECS.Components.GamePlay;
using IsoECS.GamePlay;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace IsoECS.DataRenderers
{
    public class CityInformationRenderer : DataRenderer<CityInformationComponent, Panel>
    {
        private Label population;
        private Label money;
        private Label date;

        public CityInformationRenderer(CityInformationComponent data, GameWorld world)
            : base(data, world)
        {
            Control = new Panel(Manager)
            {
                Passive = true,
                Color = Color.Transparent
            };
            Control.Init();

            Control.Left = 0;
            Control.Top = 0;
            Control.Width = Manager.GraphicsDevice.Viewport.Width;
            Control.Height = Manager.GraphicsDevice.Viewport.Height;

            Data.PopulationChanged += new CityInformationComponent.CityInformationEventHandler(Data_PopulationChanged);
            Data.TreasuryChanged += new CityInformationComponent.CityInformationEventHandler(Data_TreasuryChanged);
            World.Date.DayChanged += new GameDateComponent.GameDateEventHandler(Date_TimeChanged);
        }

        public override Panel GetControl(Control parent)
        {
            int columnWidth = 175;

            // Add the appropraite UI elements to scene
            Label lbl = new Label(Manager)
            {
                Text = "Population: ",
                Color = Color.White,
                Width = columnWidth,
                Parent = Control
            };

            population = new Label(Manager)
            {
                Text = Data.Population.ToString(),
                Color = Color.White,
                Alignment = Alignment.MiddleRight,
                Width = columnWidth,
                Parent = Control
            };

            lbl = new Label(Manager)
            {
                Text = "Treasury: ",
                Top = 15,
                Width = columnWidth,
                Parent = Control
            };

            money = new Label(Manager)
            {
                Text = Data.Treasury.ToString("c0"),
                Top = 15,
                Alignment = Alignment.MiddleRight,
                Width = columnWidth,
                Parent = Control
            };

            date = new Label(Manager)
            {
                Text = GetTimeText(),
                Width = Manager.GraphicsDevice.Viewport.Width,
                Alignment = Alignment.MiddleCenter,
                Parent = Control
            };

            Manager.Add(Control);

            return base.GetControl(parent);
        }

        public override void Shutdown()
        {
            Data.PopulationChanged -= Data_PopulationChanged;
            Data.TreasuryChanged -= Data_TreasuryChanged;
            World.Date.DayChanged -= Date_TimeChanged;

            Manager.Remove(Control);
        }

        private void Data_TreasuryChanged(CityInformationComponent sender)
        {
            money.Text = Data.Treasury.ToString("c0");
        }

        private void Data_PopulationChanged(CityInformationComponent sender)
        {
            population.Text = Data.Population.ToString();
        }

        private void Date_TimeChanged(GameDateComponent sender)
        {
            date.Text = GetTimeText();
        }

        private string GetTimeText()
        {
            return string.Format("{0} of {1}, {2} C.E.", StringHelper.Ordinal(World.Date.Day), World.Date.MonthName, World.Date.Year);
        }
    }
}
