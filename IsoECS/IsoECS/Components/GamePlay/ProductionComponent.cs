using System;
using System.Collections.Generic;
using IsoECS.Entities;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class ProductionComponent : Component
    {
        #region Events

        public delegate void ProductionEventHandler(ProductionComponent sender);
        public delegate void EmployeeEventHandler(ProductionComponent sender, Entity employee);

        public event EmployeeEventHandler EmployeeAdded;
        public event EmployeeEventHandler EmployeeRemoved;
        public event EmployeeEventHandler HaulerAdded;
        public event EmployeeEventHandler HaulerRemoved;
        public event ProductionEventHandler StageChanged;
        public event ProductionEventHandler WorkDoneChanged;

        #endregion

        #region Fields

        private List<int> employees;
        private List<int> haulers;

        private int currentStage;
        private double workDone;

        #endregion

        #region Properties

        // the recipe for this generator
        public string Recipe { get; set; }

        public int MaxEmployees { get; set; }
        public Gender EmployeeGender { get; set; }
        public int MaxHaulers { get; set; }

        public int[] Employees
        {
            get { return employees.ToArray(); }
        }

        public int[] Haulers
        {
            get { return haulers.ToArray(); }
        }

        public int CurrentStage 
        {
            get { return currentStage; }
            set
            {
                if (currentStage != value)
                {
                    currentStage = value;
                    
                    if (StageChanged != null)
                        StageChanged.Invoke(this);
                }
            }
        }
        
        public double WorkDone 
        {
            get { return workDone; }
            set
            {
                if (workDone != value)
                {
                    workDone = value;

                    if (WorkDoneChanged != null)
                        WorkDoneChanged.Invoke(this);
                }
            }
        }
        
        public long LastUpdateTime { get; set; }

        #endregion

        #region Constructors

        public ProductionComponent()
        {
            employees = new List<int>();
            haulers = new List<int>();
            EmployeeGender = Gender.BOTH;

            workDone = 0;
            currentStage = 0;
        }

        #endregion

        #region Methods

        public bool AddEmployee(Entity e)
        {
            if (employees.Count >= MaxEmployees)
                return false;

            if (employees.Contains(e.ID))
                return false;

            employees.Add(e.ID);

            if (EmployeeAdded != null)
                EmployeeAdded.Invoke(this, e);

            return true;
        }

        public void RemoveEmployee(Entity e)
        {
            if (employees.Remove(e.ID))
            {
                if (EmployeeRemoved != null)
                    EmployeeRemoved.Invoke(this, e);
            }
        }

        public bool AddHauler(Entity e)
        {
            if (haulers.Count >= MaxHaulers)
                return false;

            if (haulers.Contains(e.ID))
                return false;

            haulers.Add(e.ID);

            if (HaulerAdded != null)
                HaulerAdded.Invoke(this, e);

            return true;
        }

        public void RemoveHauler(Entity e)
        {
            if (haulers.Remove(e.ID))
            {
                if (HaulerRemoved != null)
                    HaulerRemoved.Invoke(this, e);
            }
        }

        #endregion
    }
}
