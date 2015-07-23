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
                    if (previousMovie.StartDate + previousMovie.Duration > currentMovie.StartDate || !MeetsDateTimeFilters(currentMovie))
                    {
                        overlap++;
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

        //Hard coded filters to see actual program results 
        private static bool MeetsDateTimeFilters(Movie currentMovie)
        {
            var earliestTime = new TimeSpan(9,0,0);
            var latestTime = new TimeSpan(16,25,0);

            bool duringSaturdayWorkHours = currentMovie.StartDate.DayOfWeek == DayOfWeek.Saturday && currentMovie.StartDate.TimeOfDay < new TimeSpan(19,0, 0);
            if (duringSaturdayWorkHours)
            {
                return false;
            }

            bool isASaturday = currentMovie.StartDate.DayOfWeek == DayOfWeek.Saturday;
            bool isASunday = currentMovie.StartDate.DayOfWeek == DayOfWeek.Sunday;
            bool isWeekend = isASaturday || isASunday;

            bool startsDuringWork = currentMovie.StartDate.TimeOfDay > earliestTime ;
            bool stillRunningDuringWork = (currentMovie.StartDate + currentMovie.Duration).TimeOfDay > earliestTime;
            if (!isWeekend && (startsDuringWork || stillRunningDuringWork) && currentMovie.StartDate.TimeOfDay < latestTime)
            {
                    workClash++;
                    return false;
            }
            return true;
        }
    }
}
