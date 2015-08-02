using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace FilmFestivalOrganiser
{
    public class GetMoviesFromWatchlistUrl
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
    }
}
