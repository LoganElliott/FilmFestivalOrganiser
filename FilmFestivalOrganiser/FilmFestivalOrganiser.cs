using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FilmFestivalOrganiser
{
    class FilmFestivalOrganiser
    {
        static void Main(string[] args)
        {
            const string watchlistUrl = @"http://www.nziff.co.nz/2015/auckland/wishlist/iDO/";
            var moviesDictionary = Utilities.GetMoviesFromWatchlist(watchlistUrl);
            var allMoviesWithAllTimes = Utilities.GetAllMovieTimesForWatchlistMovies(moviesDictionary);
            var setsOfMovies = Utilities.MethodTwo(new HashSet<HashSet<Movie>>(allMoviesWithAllTimes.Values));
            CheckForValidSetOfMovies(setsOfMovies);

        }

        private static int overlap = 0;
        private static int workClash = 0;
        private static int ashClash = 0;
        private static void CheckForValidSetOfMovies(IEnumerable<Movie[]> setsOfMovies)
        {
            var validMovies = new List<Movie[]>();
            int totalMovies = 0;
            foreach (var setOfMovies in setsOfMovies)
            {
                Boolean validMovieSet = true;
                var orderedMovies = setOfMovies.OrderBy(x => x.StartDate).ToArray();
                var previousMovie = orderedMovies[0];
                for (int i = 1; i < orderedMovies.Count(); i++)
                {
                    var currentMovie = orderedMovies[i];
                    if (previousMovie.StartDate + previousMovie.Duration > currentMovie.StartDate)
                    {
                        overlap++;
                        validMovieSet = false;
                        break;
                    }
                    if (!MeetsDateTimeFilters(currentMovie))
                    {
                        validMovieSet = false;
                        break;
                    }
                }
                if (validMovieSet)
                {
                    validMovies.Add(orderedMovies);
                }
                totalMovies++;
            }
        }

        private static bool MeetsDateTimeFilters(Movie currentMovie)
        {
            var earliestTime = new TimeSpan(0,9,0);
            var latestTime = new TimeSpan(0,16,30);
            bool meetsFilters = true;

            if (currentMovie.StartDate.DayOfWeek == DayOfWeek.Saturday && currentMovie.StartDate.TimeOfDay < new TimeSpan(0, 19, 0))
            {
                //meetsFilters = false;
                ashClash++;
            }

            if (currentMovie.StartDate.DayOfWeek != DayOfWeek.Sunday && currentMovie.StartDate.DayOfWeek != DayOfWeek.Saturday)
            {
                if (currentMovie.StartDate.TimeOfDay > earliestTime && currentMovie.StartDate.TimeOfDay < latestTime)
                {
                    meetsFilters = false;
                    workClash++;
                }
                else if ((currentMovie.StartDate + currentMovie.Duration).TimeOfDay > earliestTime)
                {
                    meetsFilters = false;
                    workClash++;
                }
            }
            return meetsFilters;
        }
    }
}
