using System.Collections.Generic;
using System.Linq;
using FilmFestivalOrganiser;
using Nancy;
using Newtonsoft.Json;

namespace FilmFestivalOrganiserWebService
{
    public class OrgansiserNancySite : NancyModule
    {
        
        public OrgansiserNancySite()
        {
           Get["/getWishlistJson/{wishlistId}"] = parameters =>
            {
                var wishlistUrl = @"http://www.nziff.co.nz/s/" + parameters.wishlistId;
                var moviesDictionary = GetMoviesFromWishlistUrl.GetMoviesFromWishlist(wishlistUrl);
                Dictionary<string, HashSet<Movie>> allMoviesWithAllTimes = AllMoviesWithAllTimesGenerator.GetAllMovieTimesForWishlistMovies(moviesDictionary);
                var setsOfMovies = MovieCombinationAndValidation.CalculateMovieCombinations(new HashSet<HashSet<Movie>>(allMoviesWithAllTimes.Values));
                var validMovieOrder = MovieCombinationAndValidation.CheckForValidSetOfMovies(setsOfMovies).First();
                var moviesGroupedByDay = MovieCombinationAndValidation.GroupMoviesUpByDate(validMovieOrder);
                return JsonConvert.SerializeObject(moviesGroupedByDay);
            };

            Get["/test"] = _ => "Looks like in your in the right part of town";
        }
    }
}