using System.Collections.Generic;
using System.Linq;
using FilmFestivalOrganiser;
using Nancy;

namespace FilmFestivalOrganiserWebService
{
    public class OrgansiserNancySite : NancyModule
    {
        public OrgansiserNancySite()
        {
            Get["/watchlistUrl={watchlistUrl}"] = parameters =>
            {
                //todo: take out hardcoded full url, that will be later handled be fully passed in via the angular project 
                var watchlistUrl = @"http://www.nziff.co.nz/2015/auckland/wishlist/" + parameters.watchlistUrl;
                var moviesDictionary = GetMoviesFromWatchlistUrl.GetMoviesFromWatchlist(watchlistUrl);
                Dictionary<string, HashSet<Movie>> allMoviesWithAllTimes = AllMoviesWithAllTimesGenerator.GetAllMovieTimesForWatchlistMovies(moviesDictionary);
                //todo: take "Take(4)" out set so it finishes quickly atm
                var setsOfMovies = MovieCombinationAndValidation.CalculateMovieCombinations(new HashSet<HashSet<Movie>>(allMoviesWithAllTimes.Values.Take(4)));
                var validMovieOrder = MovieCombinationAndValidation.CheckForValidSetOfMovies(setsOfMovies).First();
                return Response.AsJson(validMovieOrder);
            };

            Get["/"] = parameters =>
            {
                return "Hello World";
            };
        }
    }
}