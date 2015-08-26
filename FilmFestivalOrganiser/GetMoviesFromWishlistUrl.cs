using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace FilmFestivalOrganiser
{
    public class GetMoviesFromWishlistUrl
    {
        public static List<Movie> GetMoviesFromWishlist(string wishlistUrl)
        {
            var rawMoviesCollection = GetRawMoviesHtml(wishlistUrl);
            return ExtractwishlistMoviesFromHtml(rawMoviesCollection);
        }

        private static IEnumerable<HtmlNode> GetRawMoviesHtml(string wishlistUrl)
        {
            var web = new HtmlWeb();
            var wishListWebsitePageDocument = web.Load(wishlistUrl);
            return wishListWebsitePageDocument.DocumentNode.SelectNodes("//*[@class='" + "session-info film-info" + "']");
        }

        private static List<Movie> ExtractwishlistMoviesFromHtml(IEnumerable<HtmlNode> rawMoviesCollection)
        {
            const string mainSiteUrl = "http://www.nziff.co.nz";
            var movies = new List<Movie>();
            foreach (var movieParentNode in rawMoviesCollection)
            {
                var movieNode = movieParentNode.SelectSingleNode("h3/a");
                var movie = new Movie
                {
                    Title = System.Net.WebUtility.HtmlDecode(movieNode.InnerText),
                    WebsiteUrl = new Uri(mainSiteUrl + movieNode.Attributes["href"].Value)
                };
                    movies.Add(movie);
            }
            return movies;
        }
    }
}
