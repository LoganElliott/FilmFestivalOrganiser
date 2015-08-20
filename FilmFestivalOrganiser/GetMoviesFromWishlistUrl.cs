using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace FilmFestivalOrganiser
{
    public class GetMoviesFromWishlistUrl
    {
        public static HashSet<Movie> GetMoviesFromWishlist(string wishlistUrl)
        {
            var rawMoviesCollection = GetRawMoviesHtml(wishlistUrl);
            return ExtractwishlistMoviesFromHtml(rawMoviesCollection);
        }

        private static HtmlNodeCollection GetRawMoviesHtml(string wishlistUrl)
        {
            var web = new HtmlWeb();
            var wishListWebsitePageDocument = web.Load(wishlistUrl);
            return wishListWebsitePageDocument.DocumentNode.SelectNodes("//*[@class='" + "session-info film-info" + "']");
        }

        private static HashSet<Movie> ExtractwishlistMoviesFromHtml(HtmlNodeCollection rawMoviesCollection)
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
