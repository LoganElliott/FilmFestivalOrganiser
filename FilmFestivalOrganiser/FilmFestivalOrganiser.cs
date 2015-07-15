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
            var stopwatch = new Stopwatch();
            stopwatch.Start();
           // Utilities.CalculateAllCombinationsOfMovies(allMoviesWithAllTimes);
            stopwatch.Stop();
            var mine = stopwatch.ElapsedTicks;
            stopwatch.Restart();
           var thing = Utilities.MethodTwo(new HashSet<HashSet<Movie>>(allMoviesWithAllTimes.Values.Take(14))).Take(1000000000).ToList();
           var toms = stopwatch.ElapsedTicks;
           var diff = (double)toms/mine*100;
        }
    }
}
