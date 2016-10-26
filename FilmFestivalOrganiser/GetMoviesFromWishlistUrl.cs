using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace FilmFestivalOrganiser
{
    public class GetMoviesFromWishListUrl
    {
        private static readonly Regex _regex = new Regex(@"url\((.+)\);");
        private const string SiteUrl = @"http://www.nziff.co.nz/";

        public static Dictionary<string, Movie> GetMoviesFromWishlist(string wishlistUrl)
        {
            var web = new HtmlWeb();
            var wishListWebsitePageDocument = web.Load(wishlistUrl);
            var rawMoviesCollection = wishListWebsitePageDocument.DocumentNode.SelectNodes("//*[@class='" + "session-info film-info" + "']").ToArray();
            var rawThumbnailImages = wishListWebsitePageDocument.DocumentNode.SelectNodes("//*[@class='" + "media" + "']").ToArray();
            return ExtractwishlistMoviesFromHtml(rawMoviesCollection, rawThumbnailImages);
        }


        private static Dictionary<string, Movie> ExtractwishlistMoviesFromHtml(HtmlNode[] rawMoviesCollection, HtmlNode[] rawThumbnailImages)
        {
            var mainSiteUrl = new Uri("http://www.nziff.co.nz");
            var movies = new Dictionary<string,Movie>();
            for(int i =0;i< rawMoviesCollection.Length;i++)
            {
                var movieParentNode = rawMoviesCollection[i];
                var rawThumbnailText = rawThumbnailImages[i];

                var movieNode = movieParentNode.SelectSingleNode("h3/a");
                var movieName = movieNode.InnerText;
                var duration = movieParentNode.SelectSingleNode("p/span[@itemprop='duration']").Attributes.Last().Value.Replace("PT", "").Replace("M", "");
                var websiteUrl = new Uri(mainSiteUrl,movieNode.Attributes["href"].Value);

                var thumbnailUrlFromNzff = new Uri(SiteUrl + _regex.Match(rawThumbnailText.OuterHtml).Groups[1].Value);
                var cachedThumnailName = CacheThumbnail(WebUtility.HtmlEncode(movieName), thumbnailUrlFromNzff);

                var baseUri = new Uri("http://www.loganelliott.space/assets/images/thumbnails/");
                var cachedThumnail = new Uri(baseUri, cachedThumnailName);

                var movie = new Movie
                {
                    Title = WebUtility.HtmlDecode(movieName),
                    WebsiteUrl = websiteUrl,
                    ThumbnailUrl = cachedThumnail,
                    Duration = duration
                    
                };
                if (!movies.ContainsKey(movie.Title))
                {
                    movies.Add(movie.Title,movie);
                }
            }
            return movies;
        }

        private static string CacheThumbnail(string movieName,Uri thumbnailUrl)
        {
            var currentDir = Directory.GetParent(HttpContext.Current.Server.MapPath("")).Parent;
            movieName = Regex.Replace(movieName, @"[^0-9a-zA-Z]+", "");
            movieName = Regex.Replace(movieName, @"\s+", "");
            movieName += ".jpg";
            var cachedImageFile = Path.Combine(currentDir.FullName, "assets\\images\\thumbnails\\", movieName);
            if (!File.Exists(cachedImageFile))
            {
                using (var fs = File.Create(cachedImageFile)){}
                using (var client = new WebClient())
                {
                    client.DownloadFile(thumbnailUrl, cachedImageFile);
                }
            }
            return movieName;
        }
    }
}
