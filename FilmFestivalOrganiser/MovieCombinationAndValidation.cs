using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmFestivalOrganiser
{
    public class MovieCombinationAndValidation
    {
        public static IEnumerable<Movie[]> CalculateMovieCombinations(List<List<Movie>> inputs)
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

        public static Dictionary<string, HashSet<Movie>> ApplyMovieTimeFilter(Dictionary<string, HashSet<Movie>> allMoviesWithAllTimes,
            Dictionary<DayOfWeek, DayTimeFilter> dayTimeFilters)
        {
            var updatedAllMoviesWithAllTimes = new Dictionary<string, HashSet<Movie>>(allMoviesWithAllTimes);
            foreach (var movieAndTimes in allMoviesWithAllTimes)
            {
                foreach (var movie in movieAndTimes.Value)
                {
                    if (!MeetsDateTimeFilters(movie, dayTimeFilters[movie.StartDate.DayOfWeek]))
                    {
                        updatedAllMoviesWithAllTimes[movieAndTimes.Key].Remove(movie);
                    }
                }
            }
            return updatedAllMoviesWithAllTimes;
        }

        public static IEnumerable<Movie[]> CheckForValidSetOfMovies(IEnumerable<Movie[]> setsOfMovies, Dictionary<DayOfWeek,DayTimeFilter> dayTimeFilters)
        {
            foreach (var setOfMovies in setsOfMovies)
            {
                Boolean validMovieSet = true;
                var orderedMovies = setOfMovies.OrderBy(x => x.StartDate).ToArray();
                var previousMovie = orderedMovies[0];
                for (int i = 1; i < orderedMovies.Count(); i++)
                {
                    var currentMovie = orderedMovies[i];
                    if (previousMovie.StartDate + previousMovie.DurationForFilter > currentMovie.StartDate)
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
        public static bool MeetsDateTimeFilters(Movie currentMovie, DayTimeFilter dayTimeFilters)
        {
            var movieStartTime = currentMovie.StartDate.TimeOfDay;
            
            if (!dayTimeFilters.AllowedDay)
            {
                return false;
            }

            //Does movie start to early
            if (movieStartTime < TimeSpan.FromMilliseconds(dayTimeFilters.MinStartTime))
            {
                return false;
            }

            //Does movie start or end to late
            if (movieStartTime > TimeSpan.FromMilliseconds(dayTimeFilters.MaxEndTime) || (movieStartTime + currentMovie.DurationForFilter) > TimeSpan.FromMilliseconds(dayTimeFilters.MaxEndTime))
            {
                return false;
            }

            return true;
        }

        public static MovieDay[] GroupMoviesUpByDate(Movie[] validMovieOrder)
        {
            return validMovieOrder.GroupBy(movie => movie.StartDate.Date).Select(moviesForADay => new MovieDay(moviesForADay.ToArray())).ToArray();

        }
    }
}
