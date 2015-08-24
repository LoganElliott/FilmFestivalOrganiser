using System;
using System.Collections.Generic;
using Nancy;

namespace FilmFestivalOrganiser
{
    public class CreateFiltersForEachDay
    {
        private static readonly String[] DaysOfWeekShort = {"mon", "tue", "wed","thu","fri","sat","sun"};

        private static readonly DayOfWeek[] DaysOfWeekLong =
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday,
            DayOfWeek.Saturday, DayOfWeek.Sunday
        };

        public static Dictionary<DayOfWeek, DayTimeFilter> CreateDayFilters(Request request)
        {
            var dayTimeFilters = new Dictionary<DayOfWeek, DayTimeFilter>();
            for (int i = 0; i < DaysOfWeekShort.Length; i++)
            {
                var shortDayName = DaysOfWeekShort[i];
                var dayTimeFilter = new DayTimeFilter(DaysOfWeekLong[i]);
                string dayDisallowedRaw = request.Query[shortDayName + "_disallowed"];
                string minStartTimeRaw = request.Query[shortDayName + "_min"];
                string maxEndTimeRaw = request.Query[shortDayName + "_max"];

                CheckForDisallowedDay(dayTimeFilter, dayDisallowedRaw);

                CheckForMinStartTime(dayTimeFilter, minStartTimeRaw);

                CheckForMaxEndTime(dayTimeFilter, maxEndTimeRaw);

                dayTimeFilters.Add(DaysOfWeekLong[i], dayTimeFilter);
               
            }

            return dayTimeFilters;
        }

        private static void CheckForDisallowedDay(DayTimeFilter currentDayFilter, string dayDisallowedRaw)
        {
            if (dayDisallowedRaw != null)
            {
                bool dayDisallowed;
                var succesfull = bool.TryParse(dayDisallowedRaw, out dayDisallowed);

                if (succesfull)
                {
                    currentDayFilter.DisallowedDay = true;
                }
                else
                {
                    throw new ArgumentException("The value for Disallowed is not true or false");
                }
            }
        }

        private static void CheckForMinStartTime(DayTimeFilter currentDayFilter, string minStartTimeRaw)
        {
            if (minStartTimeRaw != null)
            {
                int minStartTime;
                var succesfull = int.TryParse(minStartTimeRaw, out minStartTime);
                if (succesfull)
                {
                    currentDayFilter.MinStartTime = TimeSpan.FromMinutes(minStartTime);
                }
                else
                {
                    throw new ArgumentException("The minimum start time specified is not a valid number");
                }
            }
        }

        private static void CheckForMaxEndTime(DayTimeFilter currentDayFilter, string maxEndTimeRaw)
        {
            if (maxEndTimeRaw != null)
            {
                int maxEndTime;
                var succesfull = int.TryParse(maxEndTimeRaw, out maxEndTime);
                if (succesfull)
                {
                    currentDayFilter.MaxEndTime = TimeSpan.FromMinutes(maxEndTime);
                }
                else
                {
                    throw new ArgumentException("The max end time specified is not a valid number");
                }

            }
            
        }
    }
}
