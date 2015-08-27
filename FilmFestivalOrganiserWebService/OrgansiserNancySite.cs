using System;
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
          
           Post["/api/getWishlistJson/{wishlistId}"] = parameters =>
            {
                StaticConfiguration.DisableErrorTraces = false;

                string wishlistUrl = @"http://www.nziff.co.nz/s/" + parameters.wishlistId;
                Dictionary<DayOfWeek, DayTimeFilter> dayTimeFilters = CreateFiltersForEachDay.CreateDayFilters(Request.Body);
                List<Movie> moviesDictionary = GetMoviesFromWishlistUrl.GetMoviesFromWishlist(wishlistUrl);
                Dictionary<string, List<Movie>> allMoviesWithAllTimes = new Dictionary<string, List<Movie>>();
                try
                {
                    allMoviesWithAllTimes = AllMoviesWithAllTimesGenerator.GetAllMovieTimesForWishlistMovies(moviesDictionary, dayTimeFilters);

                }
                catch (NoSessionFoundException e)
                {

                    return JsonConvert.SerializeObject(Enumerable.Empty<MovieDay>());
                }
                IEnumerable<Movie[]> setsOfMovies = MovieCombinationAndValidation.CalculateMovieCombinations(new List<List<Movie>>(allMoviesWithAllTimes.Values));
                Movie[] validMovieOrder = MovieCombinationAndValidation.CheckForValidSetOfMovies(setsOfMovies, dayTimeFilters).First();
                MovieDay[] moviesGroupedByDay = MovieCombinationAndValidation.GroupMoviesUpByDate(validMovieOrder);
                return JsonConvert.SerializeObject(moviesGroupedByDay);
            };

            Get["/api/test"] = _ => "Looks like in your in the right part of town";
        }
    }
}