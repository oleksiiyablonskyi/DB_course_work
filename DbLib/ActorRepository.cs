using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace progbase3
{
    public class ActorRepository
    {
        private SqliteConnection connection;

        public ActorRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        public Actor GetById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            Actor actor = new Actor();
            if (reader.Read())
            {
                actor.id = reader.GetInt64(0);
                actor.fullName = reader.GetString(1);
                actor.age = reader.GetInt32(2);
                actor.gender = reader.GetString(3);

            }
            else
            {
                actor = null;
            }
            reader.Close();
            connection.Close();
            return actor;
        }
        public bool DeleteById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            long nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged != 0;
        }
        public long Insert(Actor actor)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO actors (fullname, age, gender) 
                VALUES ($fullname, $age, $gender);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$fullname", actor.fullName);
            command.Parameters.AddWithValue("$age", actor.age);
            command.Parameters.AddWithValue("$gender", actor.gender);

            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;

        }
        private long GetCount()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM actors";
            long count = (long)command.ExecuteScalar();
            connection.Close();
            return count;
        }
        public List<Actor> GetAll()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors";
            SqliteDataReader reader = command.ExecuteReader();
            List<Actor> actors = new List<Actor>();
            while (reader.Read())
            {
                Actor actor = new Actor(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3));
                actors.Add(actor);
            }
            reader.Close();
            connection.Close();
            return actors;
        }

        public long GetTotalPages()
        {
            connection.Open();
            const long pageSize = 10;
            long total = (long)Math.Ceiling(this.GetCount() / (double)pageSize);
            connection.Close();
            return total;
        }

        public List<Actor> GetPage(long pageNumber)
        {
            connection.Open();
            pageNumber -= 1;
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors LIMIT $limit OFFSET $offset";
            command.Parameters.AddWithValue("$limit", 10);
            command.Parameters.AddWithValue("$offset", pageNumber * 10);
            SqliteDataReader reader = command.ExecuteReader();
            List<Actor> actors = new List<Actor>();
            while (reader.Read())
            {
                Actor actor = new Actor(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3));
                actors.Add(actor);
            }
            reader.Close();
            connection.Close();
            return actors;
        }
        public bool Update(long taskId, Actor actor)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE actors SET fullname = $fullname, age = $age, gender = $gender WHERE id = $id";
            command.Parameters.AddWithValue("$id", taskId);

            command.Parameters.AddWithValue("$fullname", actor.fullName);
            command.Parameters.AddWithValue("$age", actor.age);
            command.Parameters.AddWithValue("$gender", actor.gender);


            long nChanged = command.ExecuteNonQuery();

            connection.Close();
            return nChanged != 0;
        }
        public List<Movie> ActorMovies(long actorId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT movies_actors.actor_id, movies.id, movies.name, movies.genre, movies.release_date 
            FROM movies_actors, movies WHERE movies_actors.actor_id = $actor_id
            AND movies.id = movies_actors.movie_id";
            command.Parameters.AddWithValue("$actor_id", actorId);
            SqliteDataReader reader = command.ExecuteReader();

            List<Movie> movies = new List<Movie>();
            while (reader.Read())
            {
                string name = reader.GetString(2);
                string genre = reader.GetString(3);
                DateTime releaseDate = DateTime.Parse(reader.GetString(4));
                Movie newMovie = new Movie(name, genre, releaseDate);
                newMovie.id = int.Parse(reader.GetString(1));
                movies.Add(newMovie);
            }

            reader.Close();
            connection.Close();
            Actor actor = GetById(actorId);
            actor.movies = movies;
            return movies;
        }
        public SqliteConnection GetConnection()
        {
            return this.connection;
        }
    }
}
