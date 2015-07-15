namespace FilmFestivalOrganiser
{
    class Program
    {
        static void Main(string[] args)
        {
            const string watchlistUrl = @"http://www.nziff.co.nz/2015/auckland/wishlist/iDO/";
            var moviesDictionary = Utilities.GetMoviesFromWatchlist(watchlistUrl);
            var allMoviesWithAllTimes = Utilities.GetAllMovieTimesForWatchlistMovies(moviesDictionary);
        }
    }
}
