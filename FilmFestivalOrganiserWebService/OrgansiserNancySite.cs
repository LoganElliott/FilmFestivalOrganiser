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

            Post["/getWishListJson/{wishListId}"] = parameters =>
            {
                StaticConfiguration.DisableErrorTraces = false;

                string wishlistUrl = @"http://www.nziff.co.nz/s/" + parameters.wishListId;
                CultureInfo cultureInfo = new CultureInfo("" + Request.Query["locale"]);
                Dictionary<DayOfWeek, DayTimeFilter> dayTimeFilters = CreateFiltersForEachDay.CreateDayFilters(Request.Body);
                Dictionary<string, Movie> moviesDictionary = GetMoviesFromWishListUrl.GetMoviesFromWishlist(wishlistUrl);
                Dictionary<string, List<Movie>> allMoviesWithAllTimes = new Dictionary<string, List<Movie>>();
                allMoviesWithAllTimes = AllMoviesWithAllTimesGenerator.GetAllMovieTimesForWishlistMovies(moviesDictionary, dayTimeFilters);
                IEnumerable<Movie[]> setsOfMovies = MovieCombinationAndValidation.CalculateMovieCombinations(new List<List<Movie>>(allMoviesWithAllTimes.Values));
                Movie[] validMovieOrder;
                var setOfValidMovies = MovieCombinationAndValidation.CheckForValidSetOfMovies(setsOfMovies,
                    dayTimeFilters);
                try
                {
                    validMovieOrder = setOfValidMovies.FirstOrDefault();
                    if (validMovieOrder == null)
                    {
                        throw new NoSessionFoundException("There was no combination of movies found that had to clashes");
                    }                
                }
                catch (NoSessionFoundException error)
                {
                    return JsonConvert.SerializeObject(Enumerable.Empty<MovieDay>());
                }
                MovieDay[] moviesGroupedByDay = MovieCombinationAndValidation.GroupMoviesUpByDate(validMovieOrder, cultureInfo);
                return JsonConvert.SerializeObject(moviesGroupedByDay);
            };

            Get["/test"] = _ => "Looks like in your in the right part of town";
        }
    }
}