using System;
using TecsDotNet;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class CityInformationComponent : Component
    {
        #region Events

        public delegate void CityInformationEventHandler(CityInformationComponent sender);

        public event CityInformationEventHandler NameChanged;
        public event CityInformationEventHandler TreasuryChanged;
        public event CityInformationEventHandler PopulationChanged;

        #endregion

        #region Fields

        private string name;
        private int treasury;
        private int population;

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

        public int Treasury
        {
            get { return treasury; }
            set
            {
                if (treasury != value)
                {
                    treasury = value;
                    if (TreasuryChanged != null)
                        TreasuryChanged.Invoke(this);
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

        #endregion
    }
}
