using System;

namespace FilmFestivalOrganiser
{
    public class Movie
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public string Duration { get; set; }
        internal TimeSpan DurationForFilter { get; set; } 
        public string Location { get; set; }
        public Uri WebsiteUrl { get; set; }
        public Uri ThumbnailUrl { get; set; }
        public int Year { get; set; }

        public override string ToString()
        {
            return String.Format("Title: {0} Year: {1} Start Date: {2} Duration: {3} Location: {4} WebsiteUrl: {5} ThumbnailUrl: {6}",Title, Year, StartDate, Duration, Location, WebsiteUrl, ThumbnailUrl);
        }
    }
}
