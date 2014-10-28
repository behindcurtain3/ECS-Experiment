using System;
using System.Collections.Generic;
using IsoECS.Behaviors;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public enum Gender
    {
        BOTH,
        MALE,
        FEMALE
    }

    [Serializable]
    public class CitizenComponent : Component
    {
        #region Events

        public delegate void CitizenEventHandler(CitizenComponent sender);

        public event CitizenEventHandler NameChanged;
        public event CitizenEventHandler SurnameChanged;
        public event CitizenEventHandler GenderChanged;
        public event CitizenEventHandler FatherChanged;
        public event CitizenEventHandler MotherChanged;
        public event CitizenEventHandler AgeChanged;
        public event CitizenEventHandler MoneyChanged;
        public event CitizenEventHandler HousingChanged;
        public event CitizenEventHandler JobChanged;
        public event CitizenEventHandler InsideChanged;
        public event CitizenEventHandler IsHaulerChanged;

        #endregion

        #region Fields

        private string name;
        private string surname;
        private Gender gender;
        private int fatherID;
        private int motherID;
        private int age;
        private int money;
        private int houseID;
        private int jobID;
        private int insideID;
        private bool isHauler;

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

        public string Surname
        {
            get { return surname; }
            set
            {
                if (surname != value)
                {
                    surname = value;
                    if (SurnameChanged != null)
                        SurnameChanged.Invoke(this);
                }
            }
        }

        public Gender Gender
        {
            get { return gender; }
            set
            {
                if (gender != value)
                {
                    gender = value;
                    if (GenderChanged != null)
                        GenderChanged.Invoke(this);
                }
            }
        }

        public int Age
        {
            get { return age; }
            set
            {
                if (age != value)
                {
                    age = value;
                    if (AgeChanged != null)
                        AgeChanged.Invoke(this);
                }
            }
        }

        public int Money
        {
            get { return money; }
            set
            {
                if (money != value)
                {
                    money = value;
                    if (MoneyChanged != null)
                        MoneyChanged.Invoke(this);
                }
            }
        }

        public int HousingID
        {
            get { return houseID; }
            set
            {
                if (houseID != value)
                {
                    houseID = value;
                    if (HousingChanged != null)
                        HousingChanged.Invoke(this);
                }
            }
        }

        public int JobID
        {
            get { return jobID; }
            set
            {
                if (jobID != value)
                {
                    jobID = value;
                    if (JobChanged != null)
                        JobChanged.Invoke(this);
                }
            }
        }

        public int InsideID
        {
            get { return insideID; }
            set
            {
                if (insideID != value)
                {
                    insideID = value;
                    if (InsideChanged != null)
                        InsideChanged.Invoke(this);
                }
            }
        }

        public bool IsHauler
        {
            get { return isHauler; }
            set
            {
                if (isHauler != value)
                {
                    isHauler = value;
                    if (IsHaulerChanged != null)
                        IsHaulerChanged.Invoke(this);
                }
            }
        }

        public int FatherID
        {
            get { return fatherID; }
            set
            {
                if (fatherID != value)
                {
                    fatherID = value;
                    if (FatherChanged != null)
                        FatherChanged.Invoke(this);
                }
            }
        }

        public int MotherID
        {
            get { return motherID; }
            set
            {
                if (motherID != value)
                {
                    motherID = value;
                    if (MotherChanged != null)
                        MotherChanged.Invoke(this);
                }
            }
        }

        public string DisplayName
        {
            get { return string.Format("{0} {1}", Name, Surname); }
        }

        // TODO: switch to a different component + use a behavior manager/brain
        public Stack<Behavior> Behaviors { get; set; }

        #endregion

        public CitizenComponent()
        {
            FatherID = -1;
            MotherID = -1;
            HousingID = -1;
            JobID = -1;
            InsideID = -1;
            Gender = GamePlay.Gender.BOTH;
            IsHauler = false;

            Behaviors = new Stack<Behavior>();
        }
    }
}
