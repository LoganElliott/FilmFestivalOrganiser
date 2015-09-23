using System;
using System.Globalization;
using System.Linq;

namespace FilmFestivalOrganiser
{
    public class MovieDay
    {
        internal DateTime Date;
        public Movie[] Movies;
        public string DateWithLanguage;

        public MovieDay(Movie[] movies,CultureInfo cultureInfo)
        {
            Movies = movies;
            Date = movies.First().StartDate.Date;
            DateWithLanguage = Date.ToString(cultureInfo.DateTimeFormat.LongDatePattern, cultureInfo);
        }
    }
}
