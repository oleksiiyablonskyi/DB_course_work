using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace progbase3
{
    public class ReviewRepository
    {
        private SqliteConnection connection;

        public ReviewRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public Review GetById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            Review review = new Review();
            if (reader.Read())
            {
                review.id = reader.GetInt64(0);
                review.text = reader.GetString(1);
                review.grade = reader.GetInt32(2);
                review.postedAt = DateTime.Parse(reader.GetString(3));
                long movieId = reader.GetInt64(4);
                long userId = reader.GetInt64(5);
                MovieRepository movRep = new MovieRepository(connection);
                review.movie = movRep.GetById(movieId);
                UserRepository userRep = new UserRepository(connection);
                review.author = userRep.GetById(userId);
                connection.Open();
            }
            else
            {
                review = null;
            }
            reader.Close();
            connection.Close();
            return review;
        }
        public bool DeleteById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            long nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged != 0;
        }
        public long Insert(Review review)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO reviews (text, grade, posted_at, movie_id, user_id) 
                VALUES ($text, $grade, $posted_at, $movie_id, $user_id);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$text", review.text);
            command.Parameters.AddWithValue("$grade", review.grade);
            command.Parameters.AddWithValue("$posted_at", review.postedAt.ToString("o"));
            command.Parameters.AddWithValue("$movie_id", review.movie.id);
            command.Parameters.AddWithValue("$user_id", review.author.id);

            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;

        }
        private long GetCount()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM reviews";
            long count = (long)command.ExecuteScalar();
            connection.Close();
            return count;
        }
        public List<Review> GetAll()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews";
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while (reader.Read())
            {     
                connection.Open();           
                Review review = new Review(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2), DateTime.Parse(reader.GetString(3)));
                long movieId = reader.GetInt64(4);
                long userId = reader.GetInt64(5);
                review.movie = new Movie(){id = movieId};
                review.author = new User(){id = userId};
                
                
                
                reviews.Add(review);
            }
            reader.Close();
            connection.Close();

            MovieRepository movRep = new MovieRepository(connection);
            UserRepository userRep = new UserRepository(connection);
            foreach (Review rev in reviews)
            {
                rev.movie = movRep.GetById(rev.movie.id);
                rev.author = userRep.GetById(rev.author.id);
            }

            return reviews;
        }

        public long GetTotalPages()
        {
            connection.Open();
            const long pageSize = 10;
            long total = (long)Math.Ceiling(this.GetCount() / (double)pageSize);
            connection.Close();
            return total;
        }

        public List<Review> GetPage(long pageNumber)
        {
            connection.Open();
            pageNumber -= 1;
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews LIMIT $limit OFFSET $offset";
            command.Parameters.AddWithValue("$limit", 10);
            command.Parameters.AddWithValue("$offset", pageNumber * 10);
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while (reader.Read())
            {
                connection.Open();
                Review review = new Review(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2), DateTime.Parse(reader.GetString(3)));
                long movieId = reader.GetInt64(4);
                long userId = reader.GetInt64(5);
                review.movie = new Movie(){id = movieId};
                review.author = new User(){id = userId};
                
                
                reviews.Add(review);
            }
            reader.Close();
            connection.Close();

            MovieRepository movRep = new MovieRepository(connection);
            UserRepository userRep = new UserRepository(connection);
            foreach (Review rev in reviews)
            {
                rev.movie = movRep.GetById(rev.movie.id);
                rev.author = userRep.GetById(rev.author.id);
            }

            return reviews;
        }
        public bool Update(long taskId, Review review)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE reviews SET text = $text, grade = $grade, posted_at = $posted_at WHERE id = $id";
            command.Parameters.AddWithValue("$id", taskId);

            command.Parameters.AddWithValue("$text", review.text);
            command.Parameters.AddWithValue("$grade", review.grade);
            command.Parameters.AddWithValue("$posted_at", review.postedAt.ToString("o"));


            long nChanged = command.ExecuteNonQuery();

            connection.Close();
            return nChanged != 0;
        }
        public List<Review> MovieReviews(long movieId)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE movie_id = $movie_id";
            command.Parameters.AddWithValue("$movie_id", movieId);
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while (reader.Read())
            {     
                connection.Open();           
                Review review = new Review(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2), DateTime.Parse(reader.GetString(3)));
                long userId = reader.GetInt64(5);
                review.movie = new Movie(){id = movieId};
                review.author = new User(){id = userId};
                
                
                
                reviews.Add(review);
            }
            reader.Close();
            connection.Close();

            MovieRepository movRep = new MovieRepository(connection);
            UserRepository userRep = new UserRepository(connection);
            foreach (Review rev in reviews)
            {
                rev.movie = movRep.GetById(rev.movie.id);
                rev.author = userRep.GetById(rev.author.id);
            }

            return reviews;
        }
    }
}
