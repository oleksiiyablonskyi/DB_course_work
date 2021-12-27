using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace progbase3
{
    [XmlType(TypeName="movie")]
    public class Movie
    {
        public long id;
        public string name;
        public string genre;
        public DateTime releaseDate;
        public List<Review> reviews;
        public List<Actor> actors;

        public Movie(long id, string name, string genre, DateTime releaseDate)
        {
            this.id = id;
            this.name = name;
            this.genre = genre;
            this.releaseDate = releaseDate;
        }
        public Movie()
        {
            
        }
        public Movie(string name, string genre, DateTime releaseDate)
        {
            this.name = name;
            this.genre = genre;
            this.releaseDate = releaseDate;
        }

        public Movie( string name, string genre, DateTime releaseDate, List<Review> reviews, List<Actor> actors) : this(name, genre, releaseDate)
        {
            this.reviews = reviews;
            this.actors = actors;
        }

        public override string ToString()
        {
            return $"'{name}' - {genre}, {releaseDate.ToShortDateString()}";
        }
    }
}
