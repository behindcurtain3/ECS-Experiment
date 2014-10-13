using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class GameDateComponent : Component
    {
        public long Time { get; set; }

        public int MinutesPerHour { get; set; }
        public int HoursPerDay { get; set; }
        public int DaysPerMonth { get; set; }
        public int MonthsPerYear { get; set; }

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

        public long Year
        {
            get { return (int)(Time / MinutesPerYear) + 1; }
        }

    }
}
