using System;
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
            Get["/getWishlistJson/{wishlistId}"] = parameters =>
            {
                var watchlistUrl = @"http://www.nziff.co.nz/" + Request.Query["year"] + "/" + Request.Query["city"] + "/wishlist/" + parameters.wishlistId;
                var moviesDictionary = GetMoviesFromWatchlistUrl.GetMoviesFromWatchlist(watchlistUrl);
                Dictionary<string, HashSet<Movie>> allMoviesWithAllTimes = AllMoviesWithAllTimesGenerator.GetAllMovieTimesForWatchlistMovies(moviesDictionary);
                var setsOfMovies = MovieCombinationAndValidation.CalculateMovieCombinations(new HashSet<HashSet<Movie>>(allMoviesWithAllTimes.Values));
                var validMovieOrder = MovieCombinationAndValidation.CheckForValidSetOfMovies(setsOfMovies).First();
                var moviesGroupedByDay = MovieCombinationAndValidation.GroupMoviesUpByDate(validMovieOrder);
                return Response.AsJson(moviesGroupedByDay);
            };
        }
    }
}