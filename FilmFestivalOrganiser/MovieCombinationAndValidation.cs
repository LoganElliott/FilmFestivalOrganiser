using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmFestivalOrganiser
{
    public class MovieCombinationAndValidation
    {
        public static IEnumerable<Movie[]> CalculateMovieCombinations(HashSet<HashSet<Movie>> inputs)
        {
            var lengths = inputs.Select(i => i.Count).ToArray();
            var periods = new int[inputs.Count];
            periods[0] = 1;
            for (int i = 1; i < periods.Length; i++)
            {
                periods[i] = periods[i - 1] * lengths[i - 1];
            }

            var totalCombinations = periods.Last() * lengths.Last();
            var combinationEnumerators = inputs.Select((movieTimes, index) => PeriodicallyRepeat(movieTimes, periods[index]).GetEnumerator()).ToArray();

            for (long i = 0; i < totalCombinations; i++)
            {
                var validMovieCombination = combinationEnumerators.Select(movieTimes =>
                {
                    movieTimes.MoveNext();
                    return movieTimes.Current;
                }).ToArray();
                yield return validMovieCombination;
            }
        }

        private static IEnumerable<Movie> PeriodicallyRepeat(IEnumerable<Movie> movieTimes, int period)
        {
            for (; ; )
            {
                foreach (var movie in movieTimes.SelectMany(movie => Enumerable.Repeat(movie, period)))
                {
                    yield return movie;
                }
            }
        }

        public static IEnumerable<Movie[]> CheckForValidSetOfMovies(IEnumerable<Movie[]> setsOfMovies)
        {
            foreach (var setOfMovies in setsOfMovies)
            {
                Boolean validMovieSet = true;
                var orderedMovies = setOfMovies.OrderBy(x => x.StartDate).ToArray();
                var previousMovie = orderedMovies[0];
                for (int i = 1; i < orderedMovies.Count(); i++)
                {
                    var currentMovie = orderedMovies[i];
                    bool doesNotClashWithAnyOtherMovie = previousMovie.StartDate + previousMovie.Duration > currentMovie.StartDate;
                    if (doesNotClashWithAnyOtherMovie || !MeetsDateTimeFilters(currentMovie))
                    {
                        validMovieSet = false;
                        break;
                    }
                    previousMovie = orderedMovies[i];
                }
                if (validMovieSet)
                {
                    yield return orderedMovies;
                }
            }
        }

        //Hard coded filters to see actual program results 
        public static bool MeetsDateTimeFilters(Movie currentMovie)
        {
            var earliestTime = new TimeSpan(9, 0, 0);
            var latestTime = new TimeSpan(16, 25, 0);

            bool duringSaturdayWorkHours = currentMovie.StartDate.DayOfWeek == DayOfWeek.Saturday && currentMovie.StartDate.TimeOfDay < new TimeSpan(19, 0, 0);
            if (duringSaturdayWorkHours)
            {
                return false;
            }

            bool isASaturday = currentMovie.StartDate.DayOfWeek == DayOfWeek.Saturday;
            bool isASunday = currentMovie.StartDate.DayOfWeek == DayOfWeek.Sunday;
            bool isWeekend = isASaturday || isASunday;

            bool startsDuringWork = currentMovie.StartDate.TimeOfDay > earliestTime;
            bool stillRunningDuringWork = (currentMovie.StartDate + currentMovie.Duration).TimeOfDay > earliestTime;
            if (!isWeekend && (startsDuringWork || stillRunningDuringWork) && currentMovie.StartDate.TimeOfDay < latestTime)
            {
                return false;
            }
            return true;
        }
    }
}
