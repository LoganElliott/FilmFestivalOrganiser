using System;

namespace FilmFestivalOrganiser
{
    public class DayTimeFilter
    {
        public DayOfWeek DayOfWeek { get; set; }

        public long MinStartTime { get; set; }

        public long MaxEndTime { get; set; }

        public bool AllowedDay { get; set; }
    }
}
