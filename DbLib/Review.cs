using System;

namespace progbase3
{
    public class Review
    {
        public long id;
        public string text;
        public int grade;
        public DateTime postedAt;
        public Movie movie;
        public User author;

        public Review(long id, string text, int grade, DateTime postedAt)
        {
            this.id = id;
            this.text = text;
            this.grade = grade;
            this.postedAt = postedAt;
        }
        public Review()
        {
            
        }
        public Review(string text, int grade, DateTime postedAt)
        {
            this.text = text;
            this.grade = grade;
            this.postedAt = postedAt;
        }

        public Review(long id, string text, int grade, DateTime postedAt, Movie movie, User author) : this(id, text, grade, postedAt)
        {
            this.movie = movie;
            this.author = author;
        }

        public override string ToString()
        {
            return $"{grade}/10: '{text}', {postedAt.ToShortDateString()}";
        }
    }
}
