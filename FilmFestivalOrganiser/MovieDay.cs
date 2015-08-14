using System;
using System.Linq;

namespace FilmFestivalOrganiser
{
    public class MovieDay
    {
        public DateTime Date;
        public Movie[] Movies;

        public MovieDay(Movie[] movies)
        {
            Movies = movies;
            Date = movies.First().StartDate.Date;
        }
    }
}
