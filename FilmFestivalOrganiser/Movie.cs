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
            return String.Format(Title, StartDate, Duration, Location, WebsiteUrl);
        }
    }
}
