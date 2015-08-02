using System;

namespace FilmFestivalOrganiser
{
    public class Movie
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan Duration { get; set; }
        public string Location { get; set; }
        public Uri WebsiteUrl { get; set; }

        public override string ToString()
        {
            return String.Format("Title: {0} Start Date: {1} Duration: {2} Location: {3} WebsiteUrl: {4}",Title, StartDate, Duration, Location, WebsiteUrl);
        }
    }
}
