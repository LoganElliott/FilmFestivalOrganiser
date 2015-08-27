using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;

namespace FilmFestivalOrganiser
{
    public class AllMoviesWithAllTimesGenerator
    {
        public static Dictionary<string, List<Movie>> GetAllMovieTimesForWishlistMovies(List<Movie> moviesFromwishlist, Dictionary<DayOfWeek, DayTimeFilter> dayTimeFilters)
        {
            var dictionary = new Dictionary<string, List<Movie>>();
            foreach (var movie in moviesFromwishlist)
            {
                var allMovieTimes = GetAllMovieTimes(movie, dayTimeFilters);
                if (allMovieTimes.Any())
                {
                    dictionary.Add(allMovieTimes.First().Title, allMovieTimes);                    
                }
            }
            return dictionary;
        }

        private static List<Movie> GetAllMovieTimes(Movie currentwishlistMovie, Dictionary<DayOfWeek, DayTimeFilter> dayTimeFilters)
        {
            var web = new HtmlWeb();
            var movieAndTimes = new List<Movie>();
            var movieWebsiteDocument = web.Load(currentwishlistMovie.WebsiteUrl.ToString());
            var movieMetaDetails = movieWebsiteDocument.DocumentNode.SelectSingleNode("//*[@class='session-table']").SelectNodes("tr/td/table/tr/td/p");
            foreach (var movieMetaDetail in movieMetaDetails)
            {
                currentwishlistMovie.Year = int.Parse(movieWebsiteDocument.DocumentNode.SelectSingleNode("//*[@class='year']").InnerHtml);
                var movie = CreateMovie(movieMetaDetail, currentwishlistMovie);
                if (MovieCombinationAndValidation.MeetsDateTimeFilters(movie, dayTimeFilters[movie.StartDate.DayOfWeek]))
                {
                    movieAndTimes.Add(movie);
                }
            }
            return movieAndTimes;
        }

        private static Movie CreateMovie(HtmlNode movieMetaDetail, Movie movieDictionaryItem)
        {
            var location = movieMetaDetail.SelectSingleNode("span[@itemprop='location']").InnerText;
            var startDate = movieMetaDetail.SelectSingleNode("meta[@itemprop='startDate']").Attributes.Last().Value;

            
            var movie = new Movie
            {
                Title = movieDictionaryItem.Title,
                WebsiteUrl = movieDictionaryItem.WebsiteUrl,
                Location = location,
                Duration = movieDictionaryItem.Duration,
                DurationForFilter = TimeSpan.FromMinutes(Int64.Parse(movieDictionaryItem.Duration)),
                StartDate = DateTime.ParseExact(startDate, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture),
                ThumbnailUrl = movieDictionaryItem.ThumbnailUrl,
                Year = movieDictionaryItem.Year
            };
            return movie;
        }
    }
}
