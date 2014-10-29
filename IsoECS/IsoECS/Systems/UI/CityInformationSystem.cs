using System.Collections.Generic;
using System.Linq;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;
using IsoECS.Systems.Threaded;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace IsoECS.Systems.UI
{
    public class CityInformationSystem : ISystem
    {
        public GraphicsDevice Graphics { get; set; }

        private Label _populationLabel;
        private Label _populationValueLabel;
        private Label _moneyLabel;
        private Label _moneyValueLabel;
        private Label _dateLabel;

        private int _updateRate = 250;
        private int _updateCountdown;

        public void Init()
        {
            int columnWidth = 175;

            // Add the appropraite UI elements to scene
            _populationLabel = new Label(EntityManager.Instance.UI)
            {
                Text = "Population: ",
                Color = Color.White,
                Width = columnWidth
            };

            _populationValueLabel = new Label(EntityManager.Instance.UI)
            {
                Color = Color.White,
                Alignment = Alignment.MiddleRight,
                Width = columnWidth
            };

            _moneyLabel = new Label(EntityManager.Instance.UI)
            {
                Text = "Treasury: ",
                Top = 15,
                Width = columnWidth
            };

            _moneyValueLabel = new Label(EntityManager.Instance.UI)
            {
                Top = 15,
                Alignment = Alignment.MiddleRight,
                Width = columnWidth
            };

            EntityManager.Instance.UI.Add(_populationLabel);
            EntityManager.Instance.UI.Add(_populationValueLabel);
            EntityManager.Instance.UI.Add(_moneyLabel);
            EntityManager.Instance.UI.Add(_moneyValueLabel);

            _dateLabel = new Label(EntityManager.Instance.UI)
            {
                Width = Graphics.Viewport.Width,
                Alignment = Alignment.MiddleCenter
            };

            EntityManager.Instance.UI.Add(_dateLabel);

            // set to zero so it updates on the first update call
            _updateCountdown = 0;
        }

        public void Update(int dt)
        {
            _updateCountdown -= dt;

            if (_updateCountdown <= 0)
            {
                _updateCountdown += _updateRate;

                // update the city information ui elements
                EntityManager.Instance.CityInformation.Population = GetPopulation(EntityManager.Instance.Entities);

                lock (PathfinderSystem.PathsRequested)
                {
                    _populationValueLabel.Text = EntityManager.Instance.CityInformation.Population.ToString("G") + string.Format(" ({0})", PathfinderSystem.PathsRequested.Count);
                }
                _moneyValueLabel.Text = EntityManager.Instance.CityInformation.Money.ToString("C0");

                _dateLabel.Text = string.Format("{0} of {1}, {2} C.E.", StringHelper.Ordinal(EntityManager.Instance.Date.Day), EntityManager.Instance.Date.MonthName, EntityManager.Instance.Date.Year);
            }
        }

        public void Shutdown()
        {
            // Remove the UI elements from the scene
            EntityManager.Instance.UI.Remove(_populationLabel);
            EntityManager.Instance.UI.Remove(_populationValueLabel);
            EntityManager.Instance.UI.Remove(_moneyLabel);
            EntityManager.Instance.UI.Remove(_moneyValueLabel);
            EntityManager.Instance.UI.Remove(_dateLabel);
        }

        private int GetPopulation(List<Entity> entities)
        {
            return entities.Count(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); });
        }
    }
}
