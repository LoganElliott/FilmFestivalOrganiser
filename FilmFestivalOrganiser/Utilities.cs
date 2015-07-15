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

        public static void CalculateAllCombinationsOfMovies(Dictionary<string, HashSet<Movie>> allMoviesWithAllTimes)
        {
            var builtUpList = new HashSet<HashSet<Movie>>();
            foreach (var currentList in allMoviesWithAllTimes.Take(14))
            {
                HashSet<HashSet<Movie>> oldBuiltUpList = new HashSet<HashSet<Movie>>();
                if (builtUpList.Count != 0)
                {
                    oldBuiltUpList = new HashSet<HashSet<Movie>>(builtUpList.Select(i => new HashSet<Movie>(i)));
                }
                var firstItem = true;
                foreach (var currentItem in currentList.Value)
                {
                    var anyListContaining = false;

                    if (firstItem)
                    {
                        foreach (var beingBuiltList in builtUpList)
                        {
                            if (!beingBuiltList.Intersect(currentList.Value).Any())
                            {
                                beingBuiltList.Add(currentItem);
                                anyListContaining = true;
                            }
                        }
                        firstItem = false;
                    }
                    if (oldBuiltUpList.Count != 0)
                    {
                        if (!anyListContaining)
                        {
                            var backUpOldList = new HashSet<HashSet<Movie>>(oldBuiltUpList.Select(i => new HashSet<Movie>(i)));
                            foreach (var oldList in backUpOldList)
                            {
                                oldList.Add(currentItem);
                            }
                            foreach (var oldList in backUpOldList)
                            {
                                builtUpList.Add(oldList);
                            }

                        }
                    }
                    else
                    {
                        builtUpList.Add(new HashSet<Movie>(new HashSet<Movie> { currentItem }));
                    }
                }
            }
            Console.WriteLine(builtUpList.Count);
        }

        public static IEnumerable<Movie[]> MethodTwo(HashSet<HashSet<Movie>> inputs)
        {
            var lengths = inputs.Select(i => i.Count).ToArray();
            var periods = new int[inputs.Count];
            periods[0] = 1;
            for (int i = 1; i < periods.Length; i++)
            {
                periods[i] = periods[i - 1] * lengths[i - 1];
            }

            var totalCombinations = periods.Last() * lengths.Last();
            var combinationEnumerators = inputs.Select((movieTimes, index) => PeriodicallyRepeat(movieTimes, periods[index]).GetEnumerator()).ToArray();

            for (long i = 0; i < totalCombinations; i++)
            {
                var validMovieCombination = combinationEnumerators.Select(movieTimes =>
                {
                    movieTimes.MoveNext();
                    return movieTimes.Current;
                }).ToArray();
                yield return validMovieCombination;
            }
        }

        static IEnumerable<Movie> PeriodicallyRepeat(IEnumerable<Movie> movieTimes, int period)
        {
            for (; ; )
            {
                var thing = movieTimes.SelectMany(movie => Enumerable.Repeat(movie, period));
                foreach (var movie in movieTimes.SelectMany(movie => Enumerable.Repeat(movie, period)))
                {
                    yield return movie;
                }
            }
        }
    }
}
