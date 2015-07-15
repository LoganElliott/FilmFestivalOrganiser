using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;

namespace FilmFestivalOrganiser
{
    class Utilities
    {
        public static HashSet<Movie> GetMoviesFromWatchlist(string watchlistUrl)
        {
            var rawMoviesCollection = GetRawMoviesHtml(watchlistUrl);
            return ExtractWatchlistMoviesFromHtml(rawMoviesCollection);
        }

        private static HtmlNodeCollection GetRawMoviesHtml(string watchlistUrl)
        {
            var web = new HtmlWeb();
            var watchListWebsitePageDocument = web.Load(watchlistUrl);
            return watchListWebsitePageDocument.DocumentNode.SelectNodes("//*[@class='" + "session-info film-info" + "']");
        }

        private static HashSet<Movie> ExtractWatchlistMoviesFromHtml(HtmlNodeCollection rawMoviesCollection)
        {
            const string mainSiteUrl = "http://www.nziff.co.nz";
            var movies = new HashSet<Movie>();
            foreach (var movieParentNode in rawMoviesCollection)
            {
                var movieNode = movieParentNode.SelectSingleNode("h3/a");
                var movie = new Movie
                {
                    Title = movieNode.InnerText,
                    WebsiteUrl = new Uri(mainSiteUrl + movieNode.Attributes["href"].Value)
                };
                    movies.Add(movie);
            }
            return movies;
        }

        public static Dictionary<string, HashSet<Movie>> GetAllMovieTimesForWatchlistMovies(HashSet<Movie> moviesFromWatchlist)
        {
            var dictionary = new Dictionary<string, HashSet<Movie>>();
            foreach (var movie in moviesFromWatchlist)
            {
                var allMovieTimes = GetAllMovieTimes(movie);
                dictionary.Add(allMovieTimes.First().Title, allMovieTimes);
            }
            return dictionary;
        }

        private static HashSet<Movie> GetAllMovieTimes(Movie currentWatchlistMovie)
        {
            var web = new HtmlWeb();
            var movieAndTimes = new HashSet<Movie>();
            var movieWebsiteDocument = web.Load(currentWatchlistMovie.WebsiteUrl.ToString());
            var movieMetaDetails = movieWebsiteDocument.DocumentNode.SelectSingleNode("//*[@class='session-table']").SelectNodes("tr/td/table/tr/td/p");
            foreach (var movieMetaDetail in movieMetaDetails)
            {
                var movie = CreateMovie(movieMetaDetail, currentWatchlistMovie);
                movieAndTimes.Add(movie);
            }
            return movieAndTimes;
        }

        private static Movie CreateMovie(HtmlNode movieMetaDetail, Movie movieDictionaryItem)
        {
            var location = movieMetaDetail.SelectSingleNode("span[@itemprop='location']").InnerText;
            var duration = movieMetaDetail.SelectSingleNode("meta[@itemprop='duration']").Attributes.Last().Value.Replace("PT", "").Replace("M", "");
            var startDate = movieMetaDetail.SelectSingleNode("meta[@itemprop='startDate']").Attributes.Last().Value;

            var movie = new Movie
            {
                Title = movieDictionaryItem.Title,
                WebsiteUrl = movieDictionaryItem.WebsiteUrl,
                Location = location,
                Duration = TimeSpan.FromMinutes(long.Parse(duration)),
                StartDate = DateTime.ParseExact(startDate, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture)
            };

            return movie;
        }
    }
}
