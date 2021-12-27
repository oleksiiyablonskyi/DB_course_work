using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace progbase3
{
    public class MovieRepository
    {
        private SqliteConnection connection;

        public MovieRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public Movie GetById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM movies WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            Movie movie = new Movie();
            if (reader.Read())
            {
                movie.id = reader.GetInt64(0);
                movie.name = reader.GetString(1);
                movie.genre = reader.GetString(2);
                movie.releaseDate = DateTime.Parse(reader.GetString(3));

            }
            else
            {
                movie = null;
            }
            reader.Close();
            connection.Close();
            return movie;
        }
        public bool DeleteById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM movies WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            long nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged != 0;
        }
        public long Insert(Movie movie)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO movies (name, genre, release_date) 
                VALUES ($name, $genre, $release_date);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$name", movie.name);
            command.Parameters.AddWithValue("$genre", movie.genre);
            command.Parameters.AddWithValue("$release_date", movie.releaseDate.ToShortDateString());

            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;

        }
        private long GetCount()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM movies";
            long count = (long)command.ExecuteScalar();
            connection.Close();
            return count;
        }
        public List<Movie> GetAll()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM movies";
            SqliteDataReader reader = command.ExecuteReader();
            List<Movie> movies = new List<Movie>();
            while (reader.Read())
            {
                Movie movie = new Movie(reader.GetInt64(0), reader.GetString(1), reader.GetString(2), DateTime.Parse(reader.GetString(3)));
                movies.Add(movie);
            }
            reader.Close();
            connection.Close();
            return movies;
        }

        public long GetTotalPages()
        {
            connection.Open();
            const long pageSize = 10;
            long total = (long)Math.Ceiling(this.GetCount() / (double)pageSize);
            connection.Close();
            return total;
        }

        public List<Movie> GetPage(long pageNumber)
        {
            connection.Open();
            pageNumber -= 1;
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM movies LIMIT $limit OFFSET $offset";
            command.Parameters.AddWithValue("$limit", 10);
            command.Parameters.AddWithValue("$offset", pageNumber * 10);
            SqliteDataReader reader = command.ExecuteReader();
            List<Movie> movies = new List<Movie>();
            while (reader.Read())
            {
                Movie movie = new Movie(reader.GetInt64(0), reader.GetString(1), reader.GetString(2), DateTime.Parse(reader.GetString(3)));
                movies.Add(movie);
            }
            reader.Close();
            connection.Close();
            return movies;
        }
        public bool Update(long taskId, Movie movie)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE movies SET name = $name, genre = $genre, release_date = $release_date WHERE id = $id";
            command.Parameters.AddWithValue("$id", taskId);

            command.Parameters.AddWithValue("$name", movie.name);
            command.Parameters.AddWithValue("$genre", movie.genre);
            command.Parameters.AddWithValue("$release_date", movie.releaseDate.ToShortDateString());


            long nChanged = command.ExecuteNonQuery();

            connection.Close();
            return nChanged != 0;
        }
        public List<Review> GetAllReviews(Movie movie)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE movie_id = $movie_id";
            command.Parameters.AddWithValue("$movie_id", movie.id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while (reader.Read())
            {
                Review review = new Review(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2), DateTime.Parse(reader.GetString(3)));
                reviews.Add(review);
            }
            reader.Close();
            movie.reviews = reviews;
            connection.Close();
            return reviews;
        }
        public List<Actor> MovieActors(long movieId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT movies_actors.movie_id, actors.id, actors.fullname, actors.age, actors.gender 
            FROM movies_actors, actors WHERE movies_actors.movie_id = $movie_id
            AND actors.id = movies_actors.actor_id";
            command.Parameters.AddWithValue("$movie_id", movieId);
            SqliteDataReader reader = command.ExecuteReader();

            List<Actor> actors = new List<Actor>();
            while (reader.Read())
            {
                string fullName = reader.GetString(2);
                int age = reader.GetInt32(3);
                string gender = reader.GetString(4);
                Actor newActor = new Actor(fullName, age, gender);
                newActor.id = int.Parse(reader.GetString(1));
                actors.Add(newActor);
            }

            reader.Close();
            connection.Close();
            Movie movie = GetById(movieId);
            movie.actors = actors;
            return actors;
        }
        public SqliteConnection GetConnection()
        {
            return connection;
        }
    }
}
