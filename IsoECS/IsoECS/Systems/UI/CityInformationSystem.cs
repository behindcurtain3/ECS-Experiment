﻿using System.Collections.Generic;
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

        public void Init(EntityManager em)
        {
            int columnWidth = 175;

            // Add the appropraite UI elements to scene
            _populationLabel = new Label(em.UI)
            {
                Text = "Population: ",
                Color = Color.White,
                Width = columnWidth
            };

            _populationValueLabel = new Label(em.UI)
            {
                Color = Color.White,
                Alignment = Alignment.MiddleRight,
                Width = columnWidth
            };

            _moneyLabel = new Label(em.UI)
            {
                Text = "Treasury: ",
                Top = 15,
                Width = columnWidth
            };

            _moneyValueLabel = new Label(em.UI)
            {
                Top = 15,
                Alignment = Alignment.MiddleRight,
                Width = columnWidth
            };

            em.UI.Add(_populationLabel);
            em.UI.Add(_populationValueLabel);
            em.UI.Add(_moneyLabel);
            em.UI.Add(_moneyValueLabel);

            _dateLabel = new Label(em.UI)
            {
                Width = Graphics.Viewport.Width,
                Alignment = Alignment.MiddleCenter
            };

            em.UI.Add(_dateLabel);

            // set to zero so it updates on the first update call
            _updateCountdown = 0;
        }

        public void Update(EntityManager em, int dt)
        {
            _updateCountdown -= dt;

            if (_updateCountdown <= 0)
            {
                _updateCountdown += _updateRate;

                // update the city information ui elements
                em.CityInformation.Population = GetPopulation(em.Entities);

                lock (PathfinderSystem.PathsRequested)
                {
                    _populationValueLabel.Text = em.CityInformation.Population.ToString("G") + string.Format(" ({0})", PathfinderSystem.PathsRequested.Count);
                }
                _moneyValueLabel.Text = em.CityInformation.Money.ToString("C0");

                _dateLabel.Text = string.Format("{0} of {1}, {2} C.E.", StringHelper.Ordinal(em.Date.Day), em.Date.MonthName, em.Date.Year);
            }
        }

        public void Shutdown(EntityManager em)
        {
            // Remove the UI elements from the scene
            em.UI.Remove(_populationLabel);
            em.UI.Remove(_populationValueLabel);
            em.UI.Remove(_moneyLabel);
            em.UI.Remove(_moneyValueLabel);
            em.UI.Remove(_dateLabel);
        }

        private int GetPopulation(List<Entity> entities)
        {
            return entities.Count(delegate(Entity e) { return e.HasComponent<CitizenComponent>(); });
        }
    }
}
