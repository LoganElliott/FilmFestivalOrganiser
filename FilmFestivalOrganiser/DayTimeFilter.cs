using System;

namespace FilmFestivalOrganiser
{
    public class DayTimeFilter
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime MinStartTime { get; set; }
        public DateTime MaxEndTime { get; set; }

        public DayTimeFilter(DayOfWeek dayOfWeek,DateTime minStartTime,DateTime maxEndTime)
        {
            DayOfWeek = dayOfWeek;
            MinStartTime = minStartTime;
            MaxEndTime = maxEndTime;
        }
    }
}
