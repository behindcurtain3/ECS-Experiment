using System;
using TecsDotNet;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class GameDateComponent : Component
    {
        #region Events

        public delegate void GameDateEventHandler(GameDateComponent sender);
        public event GameDateEventHandler TimeChanged;
        public event GameDateEventHandler DayChanged;
        public event GameDateEventHandler MonthChanged;
        public event GameDateEventHandler YearChanged;

        #endregion

        #region Fields

        private long time;
        private int prevDay = 0;
        private int prevMonth = 0;
        private int prevYear = 0;

        #endregion

        #region Properties

        public long Time 
        {
            get { return time; }
            set
            {
                if (time != value)
                {
                    time = value;
                    if (TimeChanged != null)
                        TimeChanged.Invoke(this);
                }
            }
        }

        public int MinutesPerHour { get; set; }
        public int HoursPerDay { get; set; }
        public int DaysPerMonth { get; set; }
        public int MonthsPerYear { get; set; }
        public string[] MonthNames { get; set; }

        public int MinutesPerDay
        {
            get { return MinutesPerHour * HoursPerDay; }
        }
        public int MinutesPerMonth 
        {
            get { return MinutesPerDay * DaysPerMonth; } 
        }
        public int MinutesPerYear 
        {
            get { return MinutesPerMonth * MonthsPerYear; }
        }

        public int Minute
        {
            get { return (int)(Time % MinutesPerHour); }
        }

        public int Hour
        {
            get { return (int)((Time / MinutesPerHour) % HoursPerDay); }
        }

        public int Day
        {
            // +1 bc we start counting days at 1 not 0
            get { return (int)((Time / MinutesPerDay) % DaysPerMonth) + 1; }
        }

        public int Month
        {
            // +1 bc we start counting months at 1 not 0
            get { return (int)((Time / MinutesPerMonth) % MonthsPerYear) + 1; }
        }

        public int Year
        {
            get { return (int)(Time / MinutesPerYear) + 1; }
        }

        public string MonthName
        {
            get { return MonthNames[Month]; }
        }

        #endregion

        public GameDateComponent()
        {
            TimeChanged += new GameDateEventHandler(GameDateComponent_TimeChanged);
        }
        
        #region Methods

        public long MinutesElapsed(long timeStamp)
        {
            return Time - timeStamp;
        }

        private void GameDateComponent_TimeChanged(GameDateComponent sender)
        {
            if (prevDay != Day)
            {
                prevDay = Day;
                if (DayChanged != null)
                    DayChanged.Invoke(this);
            }

            if (prevMonth != Month)
            {
                prevMonth = Month;
                if (MonthChanged != null)
                    MonthChanged.Invoke(this);
            }

            if (prevYear != Year)
            {
                prevYear = Year;
                if (YearChanged != null)
                    YearChanged.Invoke(this);
            }
        }

        #endregion
    }
}
