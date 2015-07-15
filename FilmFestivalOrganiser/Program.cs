using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;

namespace FilmFestivalOrganiser
{
    class Program
    {
        public static int totalMovies;
        static void Main(string[] args)
        {
            string url = @"http://www.nziff.co.nz/2015/auckland/wishlist/iDO/";
            var moviesDictionary = new Dictionary<string,Movie>();
            var mainSiteUrl = "http://www.nziff.co.nz";
            var web = new HtmlWeb();
            var watchListWebsitePageDocument = web.Load(url);
            var thing = watchListWebsitePageDocument.DocumentNode.SelectNodes("//*[@class='" + "session-info film-info" + "']");
            foreach (var movieParentNode in thing)
            {
                var movieNode = movieParentNode.SelectSingleNode("h3/a");
                var movie = new Movie
                {
                    Title = movieNode.InnerText,
                    WebsiteUrl = new Uri(mainSiteUrl + movieNode.Attributes["href"].Value)
                };
                if (!moviesDictionary.ContainsKey(movie.Title))
                {
                    moviesDictionary.Add(movie.Title,movie);
                }

            }
            var movies = new Dictionary<string,HashSet<Movie>>();
            foreach (var movieDictionaryItem in moviesDictionary)
            {
                var movieWebsiteDocument = web.Load(movieDictionaryItem.Value.WebsiteUrl.ToString());
                var movieMetaDetails = movieWebsiteDocument.DocumentNode.SelectSingleNode("//*[@class='session-table']").SelectNodes("tr/td/table/tr/td/p");
               foreach (var movieMetaDetail in movieMetaDetails)
               {
                   var location = movieMetaDetail.SelectSingleNode("span[@itemprop='location']").InnerText;
                   var duration = movieMetaDetail.SelectSingleNode("meta[@itemprop='duration']").Attributes.Last().Value.Replace("PT", "").Replace("M", "");
                   var startDate = movieMetaDetail.SelectSingleNode("meta[@itemprop='startDate']").Attributes.Last().Value;

                   var movie = new Movie
                   {
                       Title = movieDictionaryItem.Value.Title,
                       WebsiteUrl = movieDictionaryItem.Value.WebsiteUrl,
                       Location = location,
                       Duration = TimeSpan.FromMinutes(long.Parse(duration)),
                       StartDate = DateTime.ParseExact(startDate, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture)
                   };

                   if (!movies.ContainsKey(movie.Title))
                   {
                       movies.Add(movie.Title,new HashSet<Movie>());
                   }
                   movies[movie.Title].Add(movie);
               }
            }
        }


        public class Movie
        {
            public string Title { get; set; }
            public DateTime StartDate { get; set; }
            public TimeSpan Duration { get; set; }
            public string Location { get; set; }
            public Uri WebsiteUrl { get; set; }
        }
    }
}
