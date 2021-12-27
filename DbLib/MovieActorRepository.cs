using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
namespace progbase3
{
    public class MovieActorRepository
    {
        private SqliteConnection connection;

        public MovieActorRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public MovieActor GetById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM movies_actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            MovieActor movieActor = new MovieActor();
            if (reader.Read())
            {
                movieActor.id = reader.GetInt64(0);
                movieActor.movieId = reader.GetInt64(1);
                movieActor.actorId = reader.GetInt64(2);

            }
            else
            {
                movieActor = null;
            }
            reader.Close();
            connection.Close();
            return movieActor;
        }
        public long GetId(long movieId, long actorId)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM movies_actors WHERE movie_id = $movie_id AND actor_id = $actor_id";
            command.Parameters.AddWithValue("$movie_id", movieId);
            command.Parameters.AddWithValue("$actor_id", actorId);
            SqliteDataReader reader = command.ExecuteReader();
            MovieActor movieActor = new MovieActor();
            if (reader.Read())
            {
                movieActor.id = reader.GetInt64(0);
                movieActor.movieId = reader.GetInt64(1);
                movieActor.actorId = reader.GetInt64(2);

            }
            else
            {
                movieActor.id = -1;
            }
            reader.Close();
            connection.Close();
            return movieActor.id;
        }
        public bool DeleteById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM movies_actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            long nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged != 0;
        }
        public long Insert(MovieActor movieActor)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO movies_actors (movie_id, actor_id) 
                VALUES ($movie_id, $actor_id);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$movie_id", movieActor.movieId);
            command.Parameters.AddWithValue("$actor_id", movieActor.actorId);


            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;

        }
        private long GetCount()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM movies_actors";
            long count = (long)command.ExecuteScalar();
            connection.Close();
            return count;
        }
        public List<MovieActor> GetAll()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM movies_actors";
            SqliteDataReader reader = command.ExecuteReader();
            List<MovieActor> moviesActors = new List<MovieActor>();
            while (reader.Read())
            {
                MovieActor movieActor = new MovieActor(reader.GetInt64(0), reader.GetInt64(1), reader.GetInt64(2));
                moviesActors.Add(movieActor);
            }
            reader.Close();
            connection.Close();
            return moviesActors;
        }

        public long GetTotalPages()
        {
            connection.Open();
            const long pageSize = 10;
            long total = (long)Math.Ceiling(this.GetCount() / (double)pageSize);
            connection.Close();
            return total;
        }

        public List<MovieActor> GetPage(long pageNumber)
        {
            connection.Open();
            pageNumber -= 1;
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM movies_actors LIMIT $limit OFFSET $offset";
            command.Parameters.AddWithValue("$limit", 10);
            command.Parameters.AddWithValue("$offset", pageNumber * 10);
            SqliteDataReader reader = command.ExecuteReader();
            List<MovieActor> moviesActors = new List<MovieActor>();
            while (reader.Read())
            {
                MovieActor movieActor = new MovieActor(reader.GetInt64(0), reader.GetInt64(1), reader.GetInt64(2));
                moviesActors.Add(movieActor);
            }
            reader.Close();
            connection.Close();
            return moviesActors;
        }
        public bool Update(long taskId, MovieActor movieActor)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE movies_actors SET movie_id = $movie_id, actor_id = $actor_id WHERE id = $id";
            command.Parameters.AddWithValue("$id", taskId);

            command.Parameters.AddWithValue("$movie_id", movieActor.movieId);
            command.Parameters.AddWithValue("$actor_id", movieActor.actorId);


            long nChanged = command.ExecuteNonQuery();

            connection.Close();
            return nChanged != 0;
        }
        
    }
}
