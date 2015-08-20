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
            var daysInTheWeek = Enum.GetValues(typeof (DayOfWeek)).OfType<DayOfWeek>().ToList();
            var shortNameDaysInTheWeek = daysInTheWeek.Select(day => new {day = day, dayShortName = day.ToString().Substring(0, 3).ToLower()});
            var valuesToLookFor = shortNameDaysInTheWeek.Select(shortName => new {day = shortName.day, min = (shortName.dayShortName + "Min"), max = (shortName.dayShortName + "Max") });
            Get["/getWishlistJson/{wishlistId}"] = parameters =>
            {
                var wishlistUrl = @"http://www.nziff.co.nz/s/" + parameters.wishlistId;
                foreach (var value in valuesToLookFor)
                {
                    new DayTimeFilter(value.day, DateTime.Parse(value.min), DateTime.Parse(value.max));
                }
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