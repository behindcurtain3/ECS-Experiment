using System;

namespace IsoECS.GamePlay
{
    [Serializable]
    public class City
    {
        #region Events

        public delegate void CityEventHandler(City city);
        public event CityEventHandler NameChanged;
        public event CityEventHandler DescriptionChanged;
        public event CityEventHandler PopulationChanged;
        public event CityEventHandler FundsChanged;

        #endregion

        #region Fields

        // city details
        private string name;
        private string description;
        private int population;
        private double funds;

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    if (NameChanged != null)
                        NameChanged.Invoke(this);
                }
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    if (DescriptionChanged != null)
                        DescriptionChanged.Invoke(this);
                }
            }
        }

        public double Funds
        {
            get { return funds; }
            set
            {
                if (funds != value)
                {
                    funds = value;
                    if (FundsChanged != null)
                        FundsChanged.Invoke(this);
                }
            }
        }

        public int Population
        {
            get { return population; }
            set
            {
                if (population != value)
                {
                    population = value;
                    if (PopulationChanged != null)
                        PopulationChanged.Invoke(this);
                }
            }
        }

        public Services Services { get; set; }

        #endregion

        #region Methods

        public City()
        {
            Services = new Services();
        }

        #endregion
    }
}
