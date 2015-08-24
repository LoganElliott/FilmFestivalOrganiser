using System;
using System.Collections.Generic;
using System.Globalization;
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
                StaticConfiguration.DisableErrorTraces = false;

                string wishlistUrl = @"http://www.nziff.co.nz/s/" + parameters.wishlistId;
                Dictionary<DayOfWeek, DayTimeFilter> dayTimeFilters = CreateFiltersForEachDay.CreateDayFilters(Request);
                HashSet<Movie> moviesDictionary = GetMoviesFromWishlistUrl.GetMoviesFromWishlist(wishlistUrl);
                Dictionary<string, HashSet<Movie>> allMoviesWithAllTimes = AllMoviesWithAllTimesGenerator.GetAllMovieTimesForWishlistMovies(moviesDictionary, dayTimeFilters);
                IEnumerable<Movie[]> setsOfMovies = MovieCombinationAndValidation.CalculateMovieCombinations(new HashSet<HashSet<Movie>>(allMoviesWithAllTimes.Values));
                Movie[] validMovieOrder = MovieCombinationAndValidation.CheckForValidSetOfMovies(setsOfMovies, dayTimeFilters).First();
                MovieDay[] moviesGroupedByDay = MovieCombinationAndValidation.GroupMoviesUpByDate(validMovieOrder);
                return JsonConvert.SerializeObject(moviesGroupedByDay);
            };

            Get["/test"] = _ => "Looks like in your in the right part of town";

            Get["/"] = _ => View["index"];
        }
    }
}