using System;

namespace FilmFestivalOrganiser
{
    public class DayTimeFilter
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan MinStartTime { get; set; }
        public TimeSpan MaxEndTime { get; set; }
        public bool DisallowedDay { get; set; }

        public DayTimeFilter(DayOfWeek dayOfWeek)
        {
            
            DayOfWeek = dayOfWeek;
            MinStartTime = new TimeSpan(0,0,0);
            MaxEndTime = new TimeSpan(24, 0, 0);
            DisallowedDay = false;
        }
    }
}
