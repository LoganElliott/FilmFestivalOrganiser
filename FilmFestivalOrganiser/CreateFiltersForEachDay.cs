using System;
using System.Collections.Generic;
using System.Linq;
using Nancy.IO;
using Newtonsoft.Json;

namespace FilmFestivalOrganiser
{
    public class CreateFiltersForEachDay
    {
        public static Dictionary<DayOfWeek, DayTimeFilter> CreateDayFilters(RequestStream body)
        {
            int sizeOfBody = (int)body.Length;
            byte[] data = new byte[sizeOfBody];
            body.Read(data, 0, sizeOfBody);
            var jsonDataAsString = System.Text.Encoding.Default.GetString(data);
            var dayTimeFilters = JsonConvert.DeserializeObject<List<DayTimeFilter>>(jsonDataAsString);
            return dayTimeFilters.ToDictionary(filter => filter.DayOfWeek, filter => filter);
        }
    }
}
