using System;

namespace FilmFestivalOrganiser
{
    public class NoSessionFoundException : Exception
    {
        public NoSessionFoundException(string message) : base(message) { }
    }
}
