using System;
using System.Collections.Generic;
using IsoECS.DataStructures.GamePlay;
using IsoECS.Entities;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class HousingComponent : Component
    {
        #region Events

        public delegate void HousingEventHandler(HousingComponent sender);

        public event HousingEventHandler TennantAdded;
        public event HousingEventHandler TennantRemoved;
        public event HousingEventHandler ProspectAdded;
        public event HousingEventHandler ProspectRemoved;
        public event HousingEventHandler RentChanged;
        public event HousingEventHandler MaxOccupantsChanged;
        public event HousingEventHandler Upgraded;
        public event HousingEventHandler Downgraded;

        #endregion

        #region Fields

        // The entity ID's for the occupants of this domicile
        private List<int> tennants { get; set; }

        // Any entities who have reserved a spot in this house
        private List<int> prospectiveTennants { get; set; }

        private int rent;
        private int maxOccupants;

        #endregion

        #region Properties

        // what kind of/level of housing is this?
        public string Category { get; set; }

        // unique id of the entity this house upgrades to
        public string UpgradesTo { get; set; }

        public UpgradeRequirements UpgradeRequirements { get; set; }

        // the max number of occupants for this domicile
        public int MaxOccupants
        {
            get { return maxOccupants; }
            set
            {
                if (maxOccupants != value)
                {
                    maxOccupants = value;

                    if (MaxOccupantsChanged != null)
                        MaxOccupantsChanged.Invoke(this);
                }
            }
        }

        public int[] Tennants
        {
            get { return tennants.ToArray(); }
        }

        public int[] Prospects
        {
            get { return prospectiveTennants.ToArray(); }
        }

        // rent per month each tennant is charged
        public int Rent 
        {
            get { return rent; }
            set
            {
                if (rent != value)
                {
                    rent = value;

                    if (RentChanged != null)
                        RentChanged.Invoke(this);
                }
            }
        }

        public int SpaceAvailable
        {
            get { return MaxOccupants - (tennants.Count + prospectiveTennants.Count); }
        }

        #endregion

        public HousingComponent()
        {
            tennants = new List<int>();
            prospectiveTennants = new List<int>();
        }

        #region Methods

        public bool AddTennant(int id)
        {
            if (tennants.Count >= MaxOccupants)
                return false;

            if (tennants.Contains(id))
                return false;

            tennants.Add(id);

            if (TennantAdded != null)
                TennantAdded.Invoke(this);

            return true;
        }

        public bool AddTennant(Entity e)
        {
            return AddTennant(e.ID);
        }

        public void RemoveTennant(Entity e)
        {
            if (tennants.Remove(e.ID))
            {
                if (TennantRemoved != null)
                    TennantRemoved.Invoke(this);
            }
        }

        public bool AddProspect(int id)
        {
            if (tennants.Count + prospectiveTennants.Count >= MaxOccupants)
                return false;

            if (prospectiveTennants.Contains(id))
                return false;

            prospectiveTennants.Add(id);

            if (ProspectAdded != null)
                ProspectAdded.Invoke(this);

            return true;
        }

        public bool AddProspect(Entity e)
        {
            return AddProspect(e.ID);
        }

        public void RemoveProspect(Entity e)
        {
            if (prospectiveTennants.Remove(e.ID))
            {
                if (ProspectRemoved != null)
                    ProspectRemoved.Invoke(this);
            }
        }

        #endregion
    }
}
